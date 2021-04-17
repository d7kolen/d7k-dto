using System;
using System.Collections.Concurrent;

namespace d7k.Dto
{
	public class DtoComplexRule<TSource> : BaseValidationRule
	{
		public ValidationRuleFactory<TSource> Factory { get; set; }
		public DtoComplex Complex { get; set; }

		ConcurrentDictionary<Type, ValidationRuleFactory<object>> m_validationCache =
			new ConcurrentDictionary<Type, ValidationRuleFactory<object>>();

		public override ValidationResult Validate(ValidationContext context, ref object value)
		{
			if (value == null)
				return null;

			var realType = Complex.GetDtoAdapterSource(value).GetType();

			var tFactory = m_validationCache.GetOrAdd(realType, tType =>
			{
				var factory = Factory.CreateBySettings<object>();
				var path = factory.RuleFor(t => t);
				Complex.Validate(path, tType);
				return factory;
			});

			return tFactory.ValidateObject(value, context);
		}
	}
}
