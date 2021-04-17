using System.Collections;
using System.Collections.Generic;

namespace d7k.Dto
{
	public static partial class RuleFactory
	{
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

		public static PathValidation<TSource, string> Length<TSource>(this PathValidation<TSource, string> validation, int length)
		{
			validation.AddValidator(new LengthBetweenRule() { MinLength = length, MaxLength = length });
			return validation;
		}

		public static PathValidation<TSource, TResult> Length<TSource, TResult>(this PathValidation<TSource, TResult> validation, int length) where TResult : IEnumerable
		{
			validation.AddValidator(new LengthBetweenRule() { MinLength = length, MaxLength = length });
			return validation;
		}

		public static PathValidation<TSource, string> FixToLower<TSource>(this PathValidation<TSource, string> validation)
		{
			validation.AddValidator(new ToLowerRule());
			return validation;
		}

		public static PathValidation<TSource, string> FixToUpper<TSource>(this PathValidation<TSource, string> validation)
		{
			validation.AddValidator(new ToUpperRule());
			return validation;
		}

		public static PathValidation<TSource, TResult> FixLength<TSource,TResult>(this PathValidation<TSource, TResult> validation, int length) 
		{
			validation.AddValidator(new MaxLength() { Length = length });
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

		public static PathValidation<TSource, string> FixFileName<TSource>(this PathValidation<TSource, string> validation)
		{
			validation.AddValidator(new FileNameRule() { LatinOnly = false });
			return validation;
		}

		public static PathValidation<TSource, string> FixLatinFileName<TSource>(this PathValidation<TSource, string> validation)
		{
			validation.AddValidator(new FileNameRule() { LatinOnly = true });
			return validation;
		}

		public static PathValidation<TSource, string> Base64<TSource>(this PathValidation<TSource, string> validation)
		{
			validation.AddValidator(new Base64Rule());
			return validation;
		}
	}
}