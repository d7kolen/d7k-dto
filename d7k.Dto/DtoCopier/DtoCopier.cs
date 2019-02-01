using d7k.Utilities.Monads;
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
		static MethodInfo s_containsMeth = typeof(HashSet<string>).GetMethod(nameof(HashSet<string>.Contains));

		ConcurrentDictionary<string, Delegate> m_copiers = new ConcurrentDictionary<string, Delegate>();

		public void Copy(object dst, object src, Type templateType, bool excludePropertiesLogic, HashSet<string> properties)
		{
			var copier = GetCopyDelegate(dst.GetType(), src.GetType(), templateType, excludePropertiesLogic);
			copier.DynamicInvoke(dst, src, properties);
		}

		/// <summary>
		/// Prepare a delegate which has method signature: void CopyMethod(DstType dst, SrcType src, HashSet/<string/> properties)<para/>
		/// The delegate can take properties NULL value. In the case it will do full copy.
		/// </summary>
		public Delegate GetCopyDelegate(Type dstType, Type srcType, Type templateType, bool excludePropertiesLogic)
		{
			Func<string, Delegate> get = null;

			get = xKey =>
			{
				//if (!templateType.IsInterface)
				//	throw new NotImplementedException($"{nameof(templateType)} should be an interface type.");

				var srcTypeProps = new DtoCopierProperties(srcType, templateType);
				var dstTypeProps = new DtoCopierProperties(dstType, templateType);

				var tKey = GetKey(dstTypeProps.PropertiesType, srcTypeProps.PropertiesType, templateType, excludePropertiesLogic);
				if (tKey != xKey)
					return m_copiers.GetOrAdd(GetKey(dstTypeProps.PropertiesType, srcTypeProps.PropertiesType, templateType, excludePropertiesLogic), get);

				return CreateDelegate(dstTypeProps, srcTypeProps, excludePropertiesLogic);
			};

			return m_copiers.GetOrAdd(GetKey(dstType, srcType, templateType, excludePropertiesLogic), get);
		}

		private static string GetKey(Type dstType, Type srcType, Type templateType, bool exclude)
		{
			return dstType.FullName + "#%$&$%#" + srcType.FullName + "#%$&$%#" + templateType.FullName + "#%$&$%#" + exclude;
		}

		private Delegate CreateDelegate(DtoCopierProperties dstType, DtoCopierProperties srcType, bool exclude)
		{
			var srcProperties = srcType.GetProperties().Where(x => x.CanRead && x.CanWrite).ToList();
			var dstProperties = dstType.GetProperties().ToDictionary(x => x.Name);

			var srcPar = Expression.Parameter(srcType.PropertiesType);
			var dstPar = Expression.Parameter(dstType.PropertiesType);
			var propertiesPar = Expression.Parameter(typeof(HashSet<string>));

			var blockList = new List<Expression>();
			foreach (var srcProp in srcProperties)
			{
				var dstProp = dstProperties.Get(srcProp.Name);

				var assignProp =
					Expression.Assign(
						Expression.Property(dstPar, dstProp),
						Expression.Property(srcPar, srcProp));

				blockList.Add(assignProp);
			}

			var updateBlockList = new List<Expression>();
			foreach (var srcProp in srcProperties)
			{
				var dstProp = dstProperties.Get(srcProp.Name);

				var assignProp =
					Expression.Assign(
						Expression.Property(dstPar, dstProp),
						Expression.Property(srcPar, srcProp));

				Expression update;
				if (exclude)
					update =
						Expression.IfThen(
							Expression.Not(Expression.Call(propertiesPar, s_containsMeth, Expression.Constant(srcProp.Name))),
							assignProp);
				else
					update =
						Expression.IfThen(
							Expression.Call(propertiesPar, s_containsMeth, Expression.Constant(srcProp.Name)),
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
	}
}
