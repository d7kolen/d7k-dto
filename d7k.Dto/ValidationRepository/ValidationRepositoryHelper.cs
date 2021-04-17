using System;

namespace d7k.Dto
{
	public static class ValidationRepositoryHelper
	{
		public static ValidationResult Validate<TSource>(this IValidationRepository repository, TSource source, Action<ValidationRuleFactory<TSource>> init)
		{
			var factory = repository.Create(source, init);

			return factory.ValidateObject(source, new ValidationContext());
		}

		public static TSource Fix<TSource>(this IValidationRepository repository, TSource source, Action<ValidationRuleFactory<TSource>> init)
		{
			var factory = repository.Create(source, init);
			return factory.Fix(source);
		}

		public static TSource FixValue<TSource>(this IValidationRepository repository, TSource value, string valueName, Action<PathValidation<object, TSource>> init)
		{
			var factory = repository.Create((object)value);
			var path = new PathValidation<object, TSource>(null, factory);
			init(path);

			object objValue = value;

			var context = new ValidationContext(valueName, value);
			var result = new ValidationResult();

			foreach (var tValidator in path.Validators)
				result.Load(tValidator.Validate(context, ref objValue));

			result.ThrowIssues();

			context.WithUpdate = true;

			foreach (var tValidator in path.Validators)
				tValidator.Validate(context, ref objValue);

			return (TSource)objValue;
		}

		#region Memory leak

		//public TSource FixValue<TSource>(Expression<Func<TSource>> getValue, Action<PathValidation<object, TSource>> init)
		//{
		//	var name = NameOf.nameof(getValue);
		//	var value = getValue.Compile().Invoke();
		//
		//	return FixValue(value, name, init);
		//}

		#endregion Memory leak
	}
}
