using System;

namespace d7k.Dto
{
	public class CastRule<TProperty, TResult> : BaseValidationRule
	{
		public Func<TProperty, TResult> CastFunc
		{ get; set; }

		public override ValidationResult Validate(ValidationContext context, ref object value)
		{
			value = CastFunc((TProperty)value);
			return null;
		}

		public PathValidation<TSource, TResult> AddToValidation<TSource>(PathValidation<TSource, TProperty> validation)
		{
			var tValidator = new PathValidation<TSource, TResult>(validation.Indexer, validation.Factory);
			tValidator.Id = validation.Id;
			tValidator.Path = validation.Path;

			tValidator.Validators = validation.Validators;
			tValidator.AddValidator(this);

			return tValidator;
		}
	}
}
