using System;
using System.Collections.Generic;
using d7k.Dto.Complex;
using d7k.Dto.Utilities;

namespace d7k.Dto
{
	/// <summary>
	/// Complex of Descriptions, Validators, Converters for helping to work with DTO structures.
	/// <seealso cref="DtoComplex.InitByNestedClasses"/>
	/// </summary>
	public class DtoComplex
	{
		DtoComplexState m_state = new DtoComplexState();
		DtoComplexCache m_copyCache;
		DtoComplexInvoker m_invoker;

		public ValidationRepository ValidationRepository { get; }

		public DtoComplex(ValidationRepository validationRepository = null)
		{
			m_copyCache = new DtoComplexCache(m_state);
			m_invoker = new DtoComplexInvoker(m_copyCache, m_state, this);

			ValidationRepository = validationRepository;
			if (validationRepository == null)
				ValidationRepository = new ValidationRepository();

			ValidationRepository.SetExtension(typeof(DtoComplex), this);
		}

		/// <summary>
		/// Load all Nested Dto STATIC classes containers which have dtoAttributes.<para/>
		/// Format of nested DTO containers should fit the InitByNestedClasses method format, because the method will load them actually.
		/// KnowAssemblyTypes parameter will help you upload assemblies which haven't uploaded yet. Never operations will do with them.
		/// When dtoAttributes parameter will has null value. Then we will use single DtoContainerAttribute for it.
		/// </summary>
		public DtoComplex ByNestedClassesWithAttributes(Type[] dtoAttributes = null, Type[] knownAssemblyTypes = null)
		{
			m_state.ByNestedClassesWithAttributes(dtoAttributes, knownAssemblyTypes);
			return this;
		}

		/// <summary>
		/// Find all types like this:<para/>
		/// class TChildClass : TBaseClass, IDtoInterface0, IDtoInterface1 { }<para/>
		/// <para/>
		/// These nested classes will use as discriptions for copying and validation rule selection.<para/>
		/// <para/>
		/// Also the method find methods described for DtoAttributes (DtoCastAttribute, DtoConvertAttribute, DtoValidateAttribute)
		/// </summary>
		public void InitByNestedClasses(params Type[] classContainers)
		{
			m_state.InitByNestedClasses(classContainers);
		}

		public TDst Copy<TDst, TSrc>(TDst dst, TSrc src)
		{
			var tDst = this.GetDtoAdapterSource(dst);
			var tSrc = this.GetDtoAdapterSource(src);

			if (tDst.GetType() == tSrc.GetType())
			{
				m_invoker.ForSame(tDst, tSrc);
				return dst;
			}

			m_invoker.ForCopiers(tDst, tSrc, null);
			m_invoker.ForGenerics(tDst, tSrc, null);
			m_invoker.ForConverters(tDst, tSrc);
			m_invoker.ForGenericsConverters(tDst, tSrc, null);

			return dst;
		}

		public TDst Update<TDst, TSrc>(TDst dst, TSrc src, params string[] updationList)
		{
			var tDst = this.GetDtoAdapterSource(dst);
			var tSrc = this.GetDtoAdapterSource(src);

			var tUpdationList = new HashSet<string>().Load(updationList);

			m_invoker.ForCopiers(tDst, tSrc, tUpdationList);
			m_invoker.ForGenerics(tDst, tSrc, tUpdationList);
			m_invoker.ForConverters(tDst, tSrc, tUpdationList);
			m_invoker.ForGenericsConverters(tDst, tSrc, tUpdationList);

			return dst;
		}

		public void Validate<TSource>(PathValidation<TSource> validation, Type sourceType)
		{
			if (sourceType == null)
				return;

			var methods = m_invoker.GetValidators(sourceType);

			foreach (var t in methods)
			{
				var tFactory = Activator.CreateInstance(t.FactoryType, validation.Factory.Repository, validation.Factory.OriginalValue);
				t.Invoke(tFactory, this);

				var tRule = Activator.CreateInstance(t.RuleType) as BaseValidationRule;
				((IComplexAdapterRule)tRule).Init(tFactory, this);

				validation.AddValidator(tRule);
			}
		}

		public TRes As<TRes>(object obj)
		{
			if (obj == null)
				throw new NullReferenceException("DTO is null.");

			obj = this.GetDtoAdapterSource(obj);

			var interf = m_copyCache.GetInterfaces(obj.GetType());

			if (interf?.Contains(typeof(TRes)) == true)
				return obj.DtoAdapter<TRes>();

			if (obj is TRes)
				return (TRes)obj;

			return default(TRes);
		}

		public TDst CastValue<TDst, TSrc>(TSrc src, out TDst dst)
		{
			m_invoker.Cast.CastValue(src, out dst);
			return dst;
		}

		public ValidationResult Validate<TSource>(TSource source, Action<ValidationRuleFactory<TSource>> init)
		{
			return ValidationRepository.Validate(source, init);
		}

		public TSource Fix<TSource>(TSource source, Action<ValidationRuleFactory<TSource>> init)
		{
			return ValidationRepository.Fix(source, init);
		}

		public TSource FixValue<TSource>(TSource value, string valueName, Action<PathValidation<object, TSource>> init)
		{
			return ValidationRepository.FixValue(value, valueName, init);
		}

		public TSource CreateDto<TSource>()
		{
			return DtoFactory.Dto<TSource>();
		}
	}
}