using System;

namespace d7k.Dto
{
	/// <summary>
	/// Default DTO container. Owner class should be STATIC.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class DtoContainerAttribute : Attribute
	{
		Type[] m_knownTypes = new Type[0];

		public DtoContainerAttribute()
		{
		}

		public DtoContainerAttribute(params Type[] knownTypes)
		{
			m_knownTypes = knownTypes;
		}
	}

	/// <summary>
	/// Available signatures:<para/>
	/// static void Validate(ValidationRuleFactory&lt;TSrc&gt; t)<para/>
	/// static void Validate(ValidationRuleFactory&lt;TSrc&gt; t, DtoComplex dto)
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class DtoValidateAttribute : Attribute { }

	/// <summary>
	/// Available signatures:<para/>
	/// static void Convert(TDst dst, TSrc src)<para/>
	/// static void Convert(TDst dst, TSrc src, DtoComplex dto)
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class DtoConvertAttribute : Attribute { }

	public static class DtoComplexHelper
	{
		public static TRes Cast<TRes>(this DtoComplex complex, object obj)
		{
			var result = complex.As<TRes>(obj);
			if (result == null)
				throw new NotImplementedException($"{obj.GetType().FullName} type doesn't implement {typeof(TRes).FullName}.");

			return result;
		}

		public static T ByNestedClasses<T>(this T source, params Type[] classContainers) where T : DtoComplex
		{
			source.InitByNestedClasses(classContainers);
			return source;
		}
	}
}
