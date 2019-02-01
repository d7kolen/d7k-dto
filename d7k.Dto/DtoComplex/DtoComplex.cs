using d7k.Utilities.Monads;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace d7k.Dto
{
	/// <summary>
	/// Complex of Descriptions, Validators, Converters for helping to work with DTO structures.
	/// <seealso cref="DtoComplex.InitByNestedClasses"/>
	/// </summary>
	public class DtoComplex
	{
		DtoComplexInitialize m_state = new DtoComplexInitialize();
		ConcurrentDictionary<Type, ValidationMethodInfo[]> m_validatorsMap = new ConcurrentDictionary<Type, ValidationMethodInfo[]>();
		CopyCache m_copyCache;

		public DtoComplex()
		{
			m_copyCache = new CopyCache(m_state.Interfaces, m_state.Converters);
		}

		/// <summary>
		/// Load all Nested Dto STATIC classes containers which has dtoAttributes.<para/>
		/// Format of nested DTO containers should fit the InitByNestedClasses method format, because the method will load them actually.
		/// KnowAssemblyTypes parameter will help you upload an assembly which were alredy not uploaded. Never operations will do with them.
		/// When dtoAttributes parameter will has null value. Then we will use single DtoContainerAttribute for it.
		/// </summary>
		public DtoComplex ByNestedClassesWithAttributes(Type[] dtoAttributes = null, Type[] knowAssemblyTypes = null)
		{
			m_state.ByNestedClassesWithAttributes(dtoAttributes, knowAssemblyTypes);
			return this;
		}

		/// <summary>
		/// Find all types like this:<para/>
		/// class TChildClass : TBaseClass, IDtoInterface0, IDtoInterface1 { }<para/>
		/// <para/>
		/// These nested classes will use as discriptions for copying and validation rule selection.<para/>
		/// <para/>
		/// Also the method find methods like these:<para/>
		/// static void ValidateMethodName(ValidationRuleFactory/<IDtoInterface/> t)<para/>
		/// static void ValidateMethodName(ValidationRuleFactory/<IDtoInterface/> t, DtoComplex dto)<para/>
		/// <para/>
		/// These methods will use for DTO validation<para/>
		/// <para/>
		/// Also the Init method try find methods like these:<para/>
		/// static void ConvertMethodName(TDst dst, TSrc src)<para/>
		/// static void ConvertMethodName(TDst dst, TSrc src, DtoComplex dto)<para/>
		/// <para/>
		/// Validate and Convert methods cannot be generic<para/>
		/// If TDst is type then it should have a default constructor
		/// </summary>
		public void InitByNestedClasses(params Type[] classContainers)
		{
			m_state.InitByNestedClasses(classContainers);
		}

		public TDst Copy<TDst, TSrc>(TDst dst, TSrc src)
		{
			var tDst = GetBaseObject(dst);
			var tSrc = GetBaseObject(src);

			if (tDst.GetType() == tSrc.GetType())
			{
				tDst.ReadFrom(tSrc, tSrc.GetType());
				return dst;
			}

			var copiers = m_copyCache.GetCopingInterfaces(tDst, tSrc);
			var converters = m_copyCache.GetConverters(tDst, tSrc);

			foreach (var t in copiers)
				tDst.ReadFrom(tSrc, t);

			foreach (var t in converters)
			{
				var adaptedDst = t.GetAdaptedDst(tDst);
				var adaptedSrc = t.GetSrcAdapter(tSrc);

				t.Invoke(adaptedDst, adaptedSrc, this);
			}

			return dst;
		}

		private static object GetBaseObject<TDst>(TDst dst)
		{
			if (dst is IDtoAdapter)
				return ((IDtoAdapter)dst).GetSource();
			return dst;
		}

		public TDst Update<TDst, TSrc>(TDst dst, TSrc src, params string[] updationList)
		{
			var tDst = GetBaseObject(dst);
			var tSrc = GetBaseObject(src);

			var copiers = m_copyCache.GetCopingInterfaces(tDst, tSrc);
			var converters = m_copyCache.GetConverters(tDst, tSrc);

			foreach (var t in copiers)
				tSrc.UpdateTo(tDst, t, updationList);

			foreach (var t in converters)
			{
				var adaptedDst = t.GetDtoDst();
				var adaptedSrc = t.GetSrcAdapter(tSrc);
				t.Invoke(adaptedDst, adaptedSrc, this);

				adaptedDst.UpdateTo(tDst, t.DstType, updationList);
			}

			return dst;
		}

		public void Validate<TSource>(PathValidation<TSource> validation, Type sourceType)
		{
			if (sourceType == null)
				return;

			var methods = GetValidators(sourceType);

			foreach (var t in methods)
			{
				var tFactory = Activator.CreateInstance(t.FactoryType, validation.Factory.Repository, validation.Factory.OriginalValue);
				t.Invoke(tFactory, this);

				var tRule = Activator.CreateInstance(t.ComplexRuleType) as BaseValidationRule;
				((IComplexAdapterRule)tRule).Init(tFactory, this);

				validation.AddValidator(tRule);
			}
		}

		private ValidationMethodInfo[] GetValidators(Type sourceType)
		{
			return m_validatorsMap.GetOrAdd(sourceType, tType =>
			{
				var validators = new List<ValidationMethodInfo>();

				var allInterfaces = m_copyCache.GetInterfaces(tType);

				var interfValidators = allInterfaces.Union(AllChildTypes(tType))
					.Select(x => m_state.Validators.Get(x))
					.Where(x => x != null);

				return interfValidators.ToArray();
			});
		}

		private IEnumerable<Type> AllChildTypes(Type type)
		{
			yield return type;
			while (type.BaseType != null)
			{
				yield return type.BaseType;
				type = type.BaseType;
			}
		}

		public TRes As<TRes>(object obj)
		{
			if (obj == null)
				throw new NullReferenceException("DTO is null.");

			obj = GetSource(obj);

			var interf = m_copyCache.GetInterfaces(obj.GetType());

			if (interf?.Contains(typeof(TRes)) == true)
				return obj.DtoAdapter<TRes>();

			if (obj is TRes)
				return (TRes)obj;

			return default(TRes);
		}

		public object GetSource(object obj)
		{
			if (obj is IDtoAdapter)
				return ((IDtoAdapter)obj).GetSource();
			return obj;
		}
	}
}
