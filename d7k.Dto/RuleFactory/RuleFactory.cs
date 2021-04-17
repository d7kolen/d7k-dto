using System;
using System.Collections.Generic;

namespace d7k.Dto
{
	public static partial class RuleFactory
	{
		public static PathValidation<TSource, TResult> Complex<TSource, TResult>(this PathValidation<TSource, TResult> validation, Action<ValidationRuleFactory<TResult>> prepareRules)
		{
			var factory = validation.Factory.CreateBySettings<TResult>();
			prepareRules(factory);

			validation.AddValidator(new ComplexRule<TResult>() { Factory = factory });

			return validation;
		}

		public static PathValidation<TSource, TResult> Complex<TSource, TResult>(this PathValidation<TSource, TResult> validation,
			ValidationRuleFactory<TResult> rules)
		{
			validation.AddValidator(new ComplexRule<TResult>()
			{
				Factory = rules
			});

			return validation;
		}

		/// <summary>
		/// getDtoType function will call never. We use the function for the TDtoResult type calculation only.
		/// </summary>
		public static PathValidation<TSource, TResult> Complex<TSource, TResult, TDtoResult>(this PathValidation<TSource, TResult> validation, Action<ValidationRuleFactory<TDtoResult>> prepareRules)
		{
			var factory = validation.Factory.CreateBySettings<TDtoResult>();
			prepareRules(factory);

			validation.AddValidator(new DtoComplexAdapterRule<TDtoResult>() { Factory = factory });

			return validation;
		}

		[Obsolete("Please use Check rule")]
		public static PathValidation<TSource, TResult> Custom<TSource, TResult>(this PathValidation<TSource, TResult> validation,
			Func<ValidationContext, TResult, IEnumerable<ValidationIssue>> ruleFunc)
		{
			validation.AddValidator(new CheckRule<TResult>()
			{
				ValidateFunc = ruleFunc
			});

			return validation;
		}

		public static PathValidation<TSource, TResult> Check<TSource, TResult>(this PathValidation<TSource, TResult> validation,
			Func<ValidationContext, TResult, IEnumerable<ValidationIssue>> ruleFunc)
		{
			validation.AddValidator(new CheckRule<TResult>()
			{
				ValidateFunc = ruleFunc
			});

			return validation;
		}

		public static PathValidation<TSource, TResult> Fix<TSource, TResult>(this PathValidation<TSource, TResult> validation, Func<TResult, TResult> fixFunc)
		{
			validation.AddValidator(new FixRule<TResult>()
			{
				FixFunc = fixFunc
			});

			return validation;
		}

		/// <summary>
		/// Don't forget to restore TProperty typr before validation finish.
		/// </summary>
		public static PathValidation<TSource, TResult> Cast<TSource, TProperty, TResult>(this PathValidation<TSource, TProperty> validation, Func<TProperty, TResult> cast)
		{
			var resultValidation = new CastRule<TProperty, TResult>()
			{
				CastFunc = cast
			}
			.AddToValidation(validation);

			return resultValidation;
		}

		public static PathValidation<TSource, TResult> ValidateDto<TSource, TResult>(this PathValidation<TSource, TResult> validation)
		{
			var factory = validation.Factory;

			var dtoComplex = factory.Repository.Extension(typeof(DtoComplex)) as DtoComplex;
			if (dtoComplex == null)
				throw new NotImplementedException($"The '{nameof(ValidateDto)}' method is available for {nameof(DtoComplex)} repository only.");

			validation.AddValidator(new DtoComplexRule<TSource>()
			{
				Complex = dtoComplex,
				Factory = factory
			});

			return validation;
		}
	}
}
