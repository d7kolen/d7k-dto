using System;
using System.Collections;

namespace d7k.Dto
{
	public static partial class RuleFactory
	{
		/// <summary>
		/// Check field on Empty.<para/>
		/// Collection with 0 length are mean Empty too. If the behavier is undesirable then please look for FixEmpty or Trim rules.
		/// They can determine a preferable result for NULL value.<para/>
		/// 0 value of int, double, decimal, bool(false) and etc base types are not Empty values.
		/// </summary>
		public static PathValidation<TSource, TProperty> NotEmpty<TSource, TProperty>(this PathValidation<TSource, TProperty> validation)
		{
			validation.AddValidator(new NotEmptyRule(typeof(TProperty)));
			return validation;
		}

		/// <summary>
		/// Set specific value if the field has default(TResult)
		/// </summary>
		public static PathValidation<TSource, TResult> FixEmpty<TSource, TResult>(this PathValidation<TSource, TResult> validation, Func<TResult> defaultValue)
		{
			validation.AddValidator(new DefaultRule(typeof(TResult)) { Default = () => defaultValue() });
			return validation;
		}

		/// <summary>
		/// Set specific value if the field has default(TResult)
		/// </summary>
		public static PathValidation<TSource, TResult> FixEmpty<TSource, TResult>(this PathValidation<TSource, TResult> validation, TResult defaultValue)
		{
			validation.AddValidator(new DefaultRule(typeof(TResult)) { Default = () => defaultValue });
			return validation;
		}

		/// <summary>
		/// Set specific value if the field has default(TResult)
		/// </summary>
		public static PathValidation<TSource, TResult> FixEmpty<TSource, TResult>(this PathValidation<TSource, TResult> validation)
		{
			var defaultValue = (TResult)NotEmptyRule.DefaultValue(typeof(TResult));
			return FixEmpty(validation, defaultValue);
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

		/// <summary>
		/// Removed all Empty values from collection field
		/// </summary>
		public static PathValidation<TSource, TResult> SkipEmpty<TSource, TResult>(this PathValidation<TSource, TResult> validation) where TResult : IEnumerable
		{
			validation.AddValidator(new SkipEmptyRule());
			return validation;
		}
	}
}
