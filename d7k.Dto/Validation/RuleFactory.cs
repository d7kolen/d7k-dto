using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace d7k.Dto
{
	public static class RuleFactory
	{
		/// <summary>
		/// Check field on Empty.<para/>
		/// Collection with 0 length are mean Empty too. If the behavier is undesirable then please look on Default or Trim rules.
		/// They can determine a preferable result for NULL value.
		/// </summary>
		public static PathValidation<TSource, TProperty> NotEmpty<TSource, TProperty>(this PathValidation<TSource, TProperty> validation)
		{
			validation.AddValidator(new NotEmptyRule());
			return validation;
		}

		public static PathValidation<TSource, TProperty> Greater<TSource, TProperty>(this PathValidation<TSource, TProperty> validation, TProperty value)
		{
			validation.AddValidator(new CompareRule.Greater() { Value = value });
			return validation;
		}

		public static PathValidation<TSource, DateTime> NewEra<TSource>(this PathValidation<TSource, DateTime> validation)
		{
			validation.AddValidator(new CompareRule.NotLesser() { Value = new DateTime(1900, 1, 1) });
			return validation;
		}

		public static PathValidation<TSource, DateTime?> NewEra<TSource>(this PathValidation<TSource, DateTime?> validation)
		{
			validation.AddValidator(new CompareRule.NotLesser() { Value = new DateTime(1900, 1, 1) });
			return validation;
		}

		public static PathValidation<TSource, DateTimeOffset> NewEra<TSource>(this PathValidation<TSource, DateTimeOffset> validation)
		{
			validation.AddValidator(new CompareRule.NotLesser() { Value = new DateTimeOffset(new DateTime(1900, 1, 1), TimeSpan.Zero) });
			return validation;
		}

		public static PathValidation<TSource, DateTimeOffset?> NewEra<TSource>(this PathValidation<TSource, DateTimeOffset?> validation)
		{
			validation.AddValidator(new CompareRule.NotLesser() { Value = new DateTimeOffset(new DateTime(1900, 1, 1), TimeSpan.Zero) });
			return validation;
		}

		public static PathValidation<TSource, TProperty> NotLesser<TSource, TProperty>(this PathValidation<TSource, TProperty> validation, TProperty value)
		{
			validation.AddValidator(new CompareRule.NotLesser() { Value = value });
			return validation;
		}

		public static PathValidation<TSource, TProperty> Lesser<TSource, TProperty>(this PathValidation<TSource, TProperty> validation, TProperty value)
		{
			validation.AddValidator(new CompareRule.Lesser() { Value = value });
			return validation;
		}

		public static PathValidation<TSource, TProperty> NotGreater<TSource, TProperty>(this PathValidation<TSource, TProperty> validation, TProperty value)
		{
			validation.AddValidator(new CompareRule.NotGreater() { Value = value });
			return validation;
		}

		public static PathValidation<TSource, DateTimeOffset> ZeroOffset<TSource>(this PathValidation<TSource, DateTimeOffset> validation)
		{
			validation.AddValidator(new ZeroOffsetRule());
			return validation;
		}

		public static PathValidation<TSource, DateTimeOffset?> ZeroOffset<TSource>(this PathValidation<TSource, DateTimeOffset?> validation)
		{
			validation.AddValidator(new ZeroOffsetRule());
			return validation;
		}

		/// <summary>
		/// Remove opening and finishing invisible chars from string. Empty string will be transformed to NULL.
		/// </summary>
		public static PathValidation<TSource, string> Trim<TSource>(this PathValidation<TSource, string> validation)
		{
			validation.AddValidator(new TrimRule());
			return validation;
		}

		/// <summary>
		/// Enumeration value will check on 0 size. In the case the value will transform to NULL.
		/// </summary>
		public static PathValidation<TSource, TProperty> Trim<TSource, TProperty>(this PathValidation<TSource, TProperty> validation) where TProperty : IEnumerable
		{
			validation.AddValidator(new TrimRule());
			return validation;
		}

		public static PathValidation<TSource, string> LengthBetween<TSource>(this PathValidation<TSource, string> validation, int? minLength, int? maxLength)
		{
			validation.AddValidator(new LengthBetweenRule() { MinLength = minLength, MaxLength = maxLength });
			return validation;
		}

		public static PathValidation<TSource, TResult> LengthBetween<TSource, TResult>(this PathValidation<TSource, TResult> validation, int? minLength, int? maxLength) where TResult : IEnumerable
		{
			validation.AddValidator(new LengthBetweenRule() { MinLength = minLength, MaxLength = maxLength });
			return validation;
		}

		public static PathValidation<TSource, string> LengthLimit<TSource>(this PathValidation<TSource, string> validation, int length)
		{
			validation.AddValidator(new LengthBetweenRule() { MinLength = length, MaxLength = length });
			return validation;
		}

		public static PathValidation<TSource, TResult> LengthLimit<TSource, TResult>(this PathValidation<TSource, TResult> validation, int length) where TResult : IEnumerable
		{
			validation.AddValidator(new LengthBetweenRule() { MinLength = length, MaxLength = length });
			return validation;
		}

		public static PathValidation<TSource, string> ToLower<TSource>(this PathValidation<TSource, string> validation)
		{
			validation.AddValidator(new ToLowerRule());
			return validation;
		}

		public static PathValidation<TSource, string> ToUpper<TSource>(this PathValidation<TSource, string> validation)
		{
			validation.AddValidator(new ToUpperRule());
			return validation;
		}

		public static PathValidation<TSource, string> Email<TSource>(this PathValidation<TSource, string> validation)
		{
			validation.AddValidator(new EmailRule());
			return validation;
		}

		public static PathValidation<TSource, string> Url<TSource>(this PathValidation<TSource, string> validation)
		{
			validation.AddValidator(new UrlRule());
			return validation;
		}

		public static PathValidation<TSource, string> FileName<TSource>(this PathValidation<TSource, string> validation)
		{
			validation.AddValidator(new FileNameRule() { LatinOnly = false });
			return validation;
		}

		public static PathValidation<TSource, string> LatinFileName<TSource>(this PathValidation<TSource, string> validation)
		{
			validation.AddValidator(new FileNameRule() { LatinOnly = true });
			return validation;
		}

		public static PathValidation<TSource, TEnume> Enum<TSource, TEnume>(this PathValidation<TSource, TEnume> validation)
		{
			validation.AddValidator(new EnumRule());
			return validation;
		}

		public static PathValidation<TSource, TResult> Forbidden<TSource, TResult>(this PathValidation<TSource, TResult> validation, params object[] items)
		{
			validation.AddValidator(new ForbiddenRule() { Items = items.ToList() });
			return validation;
		}

		public static PathValidation<TSource, TResult> Available<TSource, TResult>(this PathValidation<TSource, TResult> validation, params object[] values)
		{
			validation.AddValidator(new AvailableRule() { Values = values.ToList() });
			return validation;
		}

		public static PathValidation<TSource, string> Base64<TSource>(this PathValidation<TSource, string> validation)
		{
			validation.AddValidator(new Base64Rule());
			return validation;
		}

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

		public static PathValidation<TSource, TResult> Custom<TSource, TResult>(this PathValidation<TSource, TResult> validation,
			Func<ValidationContext, TResult, IEnumerable<ValidationIssue>> ruleFunc)
		{
			validation.AddValidator(new CustomRule<TResult>()
			{
				ValidateFunc = ruleFunc
			});

			return validation;
		}

		public static PathValidation<TSource, TResult> ValidateDto<TSource, TResult>(this PathValidation<TSource, TResult> validation, DtoComplex complex)
		{
			validation.AddValidator(new DtoComplexRule<TSource>()
			{
				Complex = complex,
				Factory = validation.Factory
			});

			return validation;
		}

		/// <summary>
		/// Removed all Empty values from collection field
		/// </summary>
		public static PathValidation<TSource, TResult> SkipEmpty<TSource, TResult>(this PathValidation<TSource, TResult> validation) where TResult : IEnumerable
		{
			validation.AddValidator(new SkipEmptyRule());
			return validation;
		}

		/// <summary>
		/// Set specific value if the field has default(TResult)
		/// </summary>
		public static PathValidation<TSource, TResult> Default<TSource, TResult>(this PathValidation<TSource, TResult> validation, Func<TResult> defaultValue)
		{
			validation.AddValidator(new DefaultRule() { Default = () => defaultValue() });
			return validation;
		}

		/// <summary>
		/// Set specific value if the field has default(TResult)
		/// </summary>
		public static PathValidation<TSource, TResult> Default<TSource, TResult>(this PathValidation<TSource, TResult> validation)
		{
			if (typeof(TResult).IsArray)
				validation.AddValidator(new DefaultRule() { Default = () => Activator.CreateInstance(typeof(TResult), 0) });
			else
				validation.AddValidator(new DefaultRule() { Default = () => Activator.CreateInstance<TResult>() });

			return validation;
		}
	}
}
