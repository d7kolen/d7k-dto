using System.Linq;

namespace d7k.Dto
{
	public static partial class RuleFactory
	{
		public static PathValidation<TSource, TProperty> Greater<TSource, TProperty>(this PathValidation<TSource, TProperty> validation, TProperty minValue)
		{
			validation.AddValidator(new CompareRule.Greater() { Value = minValue });
			return validation;
		}

		public static PathValidation<TSource, TProperty> NotLesser<TSource, TProperty>(this PathValidation<TSource, TProperty> validation, TProperty minValue)
		{
			validation.AddValidator(new CompareRule.NotLesser() { Value = minValue });
			return validation;
		}

		/// <summary>
		/// Correct value to min value if a value lesser then the min value
		/// </summary>
		public static PathValidation<TSource, TProperty> FixLesser<TSource, TProperty>(this PathValidation<TSource, TProperty> validation, TProperty minValue)
		{
			validation.AddValidator(new CompareRule.NotLesser() { Value = minValue, FixToValue = true });
			return validation;
		}

		public static PathValidation<TSource, TProperty> Lesser<TSource, TProperty>(this PathValidation<TSource, TProperty> validation, TProperty maxValue)
		{
			validation.AddValidator(new CompareRule.Lesser() { Value = maxValue });
			return validation;
		}

		public static PathValidation<TSource, TProperty> NotGreater<TSource, TProperty>(this PathValidation<TSource, TProperty> validation, TProperty maxValue)
		{
			validation.AddValidator(new CompareRule.NotGreater() { Value = maxValue });
			return validation;
		}

		/// <summary>
		/// Correct value to max value if a value greater then the max value
		/// </summary>
		public static PathValidation<TSource, TProperty> FixGreater<TSource, TProperty>(this PathValidation<TSource, TProperty> validation, TProperty maxValue)
		{
			validation.AddValidator(new CompareRule.NotGreater() { Value = maxValue, FixToValue = true });
			return validation;
		}

		public static PathValidation<TSource, TResult> RoundMantissa<TSource, TResult>(this PathValidation<TSource, TResult> validation, int length)
		{
			validation.AddValidator(new MantissaLength() { Length = length });
			return validation;
		}

		public static PathValidation<TSource, TEnume> Enum<TSource, TEnume>(this PathValidation<TSource, TEnume> validation)
		{
			validation.AddValidator(new EnumRule());
			return validation;
		}

		public static PathValidation<TSource, TResult> Forbidden<TSource, TResult>(this PathValidation<TSource, TResult> validation, object[] items)
		{
			validation.AddValidator(new ForbiddenRule() { Items = items.ToList() });
			return validation;
		}

		public static PathValidation<TSource, TResult> Forbidden<TSource, TResult>(this PathValidation<TSource, TResult> validation, params TResult[] items)
		{
			validation.AddValidator(new ForbiddenRule() { Items = items.Cast<object>().ToList() });
			return validation;
		}

		public static PathValidation<TSource, TResult> Available<TSource, TResult>(this PathValidation<TSource, TResult> validation, object[] values)
		{
			validation.AddValidator(new AvailableRule() { Values = values.ToList() });
			return validation;
		}

		public static PathValidation<TSource, TResult> Available<TSource, TResult>(this PathValidation<TSource, TResult> validation, params TResult[] values)
		{
			validation.AddValidator(new AvailableRule() { Values = values.Cast<object>().ToList() });
			return validation;
		}
	}
}
