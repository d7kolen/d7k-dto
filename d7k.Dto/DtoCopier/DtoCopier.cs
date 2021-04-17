using d7k.Dto.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace d7k.Dto
{
	public class DtoCopier
	{
		static MethodInfo s_containsMethod = typeof(HashSet<string>).GetMethod(nameof(HashSet<string>.Contains));

		ConcurrentDictionary<string, Delegate> m_copiers = new ConcurrentDictionary<string, Delegate>();
		DtoCopierCastStorage m_casts;

		public DtoCopier() { }

		public DtoCopier(DtoCopierCastStorage casts) : this()
		{
			m_casts = casts;
		}

		public void Copy(object dst, Type dstTemplate, object src, Type srcTemplate, bool excludePropertiesLogic, HashSet<string> properties)
		{
			var copier = GetCopyDelegate(dst.GetType(), dstTemplate, src.GetType(), srcTemplate, excludePropertiesLogic);
			copier.DynamicInvoke(dst, src, properties);
		}

		public void Copy(object dst, object src, Type templateType, bool excludePropertiesLogic, HashSet<string> properties)
		{
			var copier = GetCopyDelegate(dst.GetType(), templateType, src.GetType(), templateType, excludePropertiesLogic);
			copier.DynamicInvoke(dst, src, properties);
		}

		/// <summary>
		/// Prepare a delegate which has method signature: void CopyMethod(DstType dst, SrcType src, HashSet/<string/> properties)<para/>
		/// The delegate can take properties NULL value. In the case it will do full copy.
		/// </summary>
		public Delegate GetCopyDelegate(Type dstType, Type dstTemplate, Type srcType, Type srcTemplate, bool excludePropertiesLogic)
		{
			Func<string, Delegate> get = null;

			get = xKey =>
			{
				var srcTypeProps = new DtoCopierProperties(srcType, srcTemplate);
				var dstTypeProps = new DtoCopierProperties(dstType, dstTemplate);

				var tKey = GetKey(dstTypeProps.PropertiesType, dstTemplate, srcTypeProps.PropertiesType, srcTemplate, excludePropertiesLogic);
				if (tKey != xKey)
					return m_copiers.GetOrAdd(GetKey(dstTypeProps.PropertiesType, dstTemplate, srcTypeProps.PropertiesType, srcTemplate, excludePropertiesLogic), get);

				return CreateDelegate(dstTypeProps, srcTypeProps, excludePropertiesLogic);
			};

			return m_copiers.GetOrAdd(GetKey(dstType, dstTemplate, srcType, srcTemplate, excludePropertiesLogic), get);
		}

		private static string GetKey(Type dstType, Type dstTemplate, Type srcType, Type srcTemplate, bool exclude)
		{
			return dstType.FullName + "#%$&$%#" + dstTemplate.FullName + "#%$&$%#" + srcType.FullName + "#%$&$%#" + srcTemplate.FullName + "#%$&$%#" + exclude;
		}

		private Delegate CreateDelegate(DtoCopierProperties dstType, DtoCopierProperties srcType, bool exclude)
		{
			AssertCopy(dstType, srcType);

			var srcProperties = srcType.GetProperties().Where(x => x.CanRead && x.CanWrite).ToList();
			var dstProperties = new Dictionary<string, PropertyInfo>().Load(dstType.GetProperties(), x => x.Name);

			var srcPar = Expression.Parameter(srcType.PropertiesType);
			var dstPar = Expression.Parameter(dstType.PropertiesType);
			var propertiesPar = Expression.Parameter(typeof(HashSet<string>));

			var blockList = new List<Expression>();
			foreach (var srcProp in srcProperties)
			{
				var dstProp = dstProperties.Get(srcProp.Name);

				var assignProp = CreateAssign(dstType, dstPar, dstProp, srcPar, srcProp);

				blockList.Add(assignProp);
			}

			var updateBlockList = new List<Expression>();
			foreach (var srcProp in srcProperties)
			{
				var dstProp = dstProperties.Get(srcProp.Name);

				var assignProp = CreateAssign(dstType, dstPar, dstProp, srcPar, srcProp);

				Expression update;
				if (exclude)
					update =
						Expression.IfThen(
							Expression.Not(Expression.Call(propertiesPar, s_containsMethod, Expression.Constant(srcProp.Name))),
							assignProp);
				else
					update =
						Expression.IfThen(
							Expression.Call(propertiesPar, s_containsMethod, Expression.Constant(srcProp.Name)),
							assignProp);

				updateBlockList.Add(update);
			}

			if (!blockList.Any())
			{
				Action<object, object, object> stub = (x, y, z) => { };
				return stub;
			}

			var body = Expression.IfThenElse(
				Expression.Equal(propertiesPar, Expression.Constant(null)),
				Expression.Block(blockList),
				Expression.Block(updateBlockList));

			return Expression.Lambda(body, dstPar, srcPar, propertiesPar).Compile();
		}

		private BinaryExpression CreateAssign(DtoCopierProperties dstType, ParameterExpression dstParameter, PropertyInfo dstProperty, ParameterExpression srcParameter, PropertyInfo srcProperty)
		{
			Expression srcValue = Expression.Property(srcParameter, srcProperty);
			if (srcProperty.PropertyType != dstProperty.PropertyType)
			{
				var cast = m_casts?.Get(dstProperty.PropertyType, srcProperty.PropertyType, dstType.GenericInterfType);
				if (cast != null)
					srcValue = Expression.Call(cast, srcValue);
				else
					srcValue = Expression.Convert(srcValue, dstProperty.PropertyType);
			}

			return Expression.Assign(
				Expression.Property(dstParameter, dstProperty),
				srcValue);
		}

		private static void AssertCopy(DtoCopierProperties dstType, DtoCopierProperties srcType)
		{
			if (dstType.InterfType == srcType.InterfType)
				return;

			if (dstType.GenericInterfType == srcType.GenericInterfType)
				return;

			throw new NotImplementedException($"Templates {dstType.PropertiesType.FullName} and {srcType.PropertiesType.FullName} are not compatible.");
		}
	}
}
