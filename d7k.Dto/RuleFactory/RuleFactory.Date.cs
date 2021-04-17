using System;

namespace d7k.Dto
{
	public static partial class RuleFactory
	{
		/// <summary>
		/// Validate that value greate then 1900-01-01
		/// </summary>
		public static PathValidation<TSource, DateTime> NewEra<TSource>(this PathValidation<TSource, DateTime> validation)
		{
			validation.AddValidator(new CompareRule.NotLesser() { Value = new DateTime(1900, 1, 1) });
			return validation;
		}

		/// <summary>
		/// Validate that value greate then 1900-01-01
		/// </summary>
		public static PathValidation<TSource, DateTime?> NewEra<TSource>(this PathValidation<TSource, DateTime?> validation)
		{
			validation.AddValidator(new CompareRule.NotLesser() { Value = new DateTime(1900, 1, 1) });
			return validation;
		}

		/// <summary>
		/// Validate that value greate then 1900-01-01
		/// </summary>
		public static PathValidation<TSource, DateTimeOffset> NewEra<TSource>(this PathValidation<TSource, DateTimeOffset> validation)
		{
			validation.AddValidator(new CompareRule.NotLesser() { Value = new DateTimeOffset(new DateTime(1900, 1, 1), TimeSpan.Zero) });
			return validation;
		}

		/// <summary>
		/// Validate that value greate then 1900-01-01
		/// </summary>
		public static PathValidation<TSource, DateTimeOffset?> NewEra<TSource>(this PathValidation<TSource, DateTimeOffset?> validation)
		{
			validation.AddValidator(new CompareRule.NotLesser() { Value = new DateTimeOffset(new DateTime(1900, 1, 1), TimeSpan.Zero) });
			return validation;
		}

		public static PathValidation<TSource, DateTime> EarlierNow<TSource>(this PathValidation<TSource, DateTime> validation)
		{
			validation.AddValidator(new CompareNowRule.Earlier());
			return validation;
		}

		public static PathValidation<TSource, DateTimeOffset> EarlierNow<TSource>(this PathValidation<TSource, DateTimeOffset> validation)
		{
			validation.AddValidator(new CompareNowRule.Earlier());
			return validation;
		}

		public static PathValidation<TSource, DateTime?> EarlierNow<TSource>(this PathValidation<TSource, DateTime?> validation)
		{
			validation.AddValidator(new CompareNowRule.Earlier());
			return validation;
		}

		public static PathValidation<TSource, DateTimeOffset?> EarlierNow<TSource>(this PathValidation<TSource, DateTimeOffset?> validation)
		{
			validation.AddValidator(new CompareNowRule.Earlier());
			return validation;
		}

		/// <summary>
		/// Set DateTime.UtcNow if a value earlear that UtcNow
		/// </summary>
		public static PathValidation<TSource, DateTime> FixEarlierNow<TSource>(this PathValidation<TSource, DateTime> validation)
		{
			validation.AddValidator(new CompareNowRule.Later() { FixToNow = true });
			return validation;
		}

		/// <summary>
		/// Set DateTime.UtcNow if a value earlear that UtcNow
		/// </summary>
		public static PathValidation<TSource, DateTimeOffset> FixEarlierNow<TSource>(this PathValidation<TSource, DateTimeOffset> validation)
		{
			validation.AddValidator(new CompareNowRule.Later() { FixToNow = true });
			return validation;
		}

		/// <summary>
		/// Set DateTime.UtcNow if a value earlear that UtcNow
		/// </summary>
		public static PathValidation<TSource, DateTime?> FixEarlierNow<TSource>(this PathValidation<TSource, DateTime?> validation)
		{
			validation.AddValidator(new CompareNowRule.Later() { FixToNow = true });
			return validation;
		}

		/// <summary>
		/// Set DateTime.UtcNow if a value earlear that UtcNow
		/// </summary>
		public static PathValidation<TSource, DateTimeOffset?> FixEarlierNow<TSource>(this PathValidation<TSource, DateTimeOffset?> validation)
		{
			validation.AddValidator(new CompareNowRule.Later() { FixToNow = true });
			return validation;
		}

		public static PathValidation<TSource, DateTime> LaterNow<TSource>(this PathValidation<TSource, DateTime> validation)
		{
			validation.AddValidator(new CompareNowRule.Later());
			return validation;
		}

		public static PathValidation<TSource, DateTimeOffset> LaterNow<TSource>(this PathValidation<TSource, DateTimeOffset> validation)
		{
			validation.AddValidator(new CompareNowRule.Later());
			return validation;
		}

		public static PathValidation<TSource, DateTime?> LaterNow<TSource>(this PathValidation<TSource, DateTime?> validation)
		{
			validation.AddValidator(new CompareNowRule.Later());
			return validation;
		}

		public static PathValidation<TSource, DateTimeOffset?> LaterNow<TSource>(this PathValidation<TSource, DateTimeOffset?> validation)
		{
			validation.AddValidator(new CompareNowRule.Later());
			return validation;
		}

		/// <summary>
		/// Set DateTime.UtcNow if a value later that UtcNow
		/// </summary>
		public static PathValidation<TSource, DateTime> FixLaterNow<TSource>(this PathValidation<TSource, DateTime> validation)
		{
			validation.AddValidator(new CompareNowRule.Earlier() { FixToNow = true });
			return validation;
		}

		/// <summary>
		/// Set DateTime.UtcNow if a value later that UtcNow
		/// </summary>
		public static PathValidation<TSource, DateTimeOffset> FixLaterNow<TSource>(this PathValidation<TSource, DateTimeOffset> validation)
		{
			validation.AddValidator(new CompareNowRule.Earlier() { FixToNow = true });
			return validation;
		}

		/// <summary>
		/// Set DateTime.UtcNow if a value later that UtcNow
		/// </summary>
		public static PathValidation<TSource, DateTime?> FixLaterNow<TSource>(this PathValidation<TSource, DateTime?> validation)
		{
			validation.AddValidator(new CompareNowRule.Earlier() { FixToNow = true });
			return validation;
		}

		/// <summary>
		/// Set DateTime.UtcNow if a value later that UtcNow
		/// </summary>
		public static PathValidation<TSource, DateTimeOffset?> FixLaterNow<TSource>(this PathValidation<TSource, DateTimeOffset?> validation)
		{
			validation.AddValidator(new CompareNowRule.Earlier() { FixToNow = true });
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
	}
}
