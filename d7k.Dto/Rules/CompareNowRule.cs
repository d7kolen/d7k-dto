using System;

namespace d7k.Dto
{
	public abstract class CompareNowRule : BaseValidationRule
	{
		public bool FixToNow { get; set; } = false;

		DateTime ToUniversalTime(object value)
		{
			if (value is DateTime)
				return ((DateTime)value).ToUniversalTime();

			if (value is DateTimeOffset)
				return ((DateTimeOffset)value).ToUniversalTime().DateTime;

			throw new NotImplementedException();
		}

		object PrepareNow(object value, DateTime now)
		{
			if (value is DateTime)
				return now;
			else if (value is DateTimeOffset)
				return new DateTimeOffset(now);

			throw new NotImplementedException();
		}

		public abstract bool IsValid(DateTime value, DateTime now);

		public abstract ValidationIssue CreateIssue(ValidationContext context);

		public override ValidationResult Validate(ValidationContext context, ref object value)
		{
			if (value == null)
				return null;

			var now = DateTime.UtcNow;

			if (IsValid(ToUniversalTime(value), now))
				return null;

			if (FixToNow)
			{
				value = PrepareNow(value, now);
				return null;
			}
			else
				return CreateIssue(context).ToResult();
		}

		public class Earlier : CompareNowRule
		{
			public override bool IsValid(DateTime value, DateTime now)
			{
				return value <= now;
			}

			public override ValidationIssue CreateIssue(ValidationContext context)
			{
				return context.Issue($"'{context.ValuePath}' should be earlier NOW.");
			}
		}

		public class Later : CompareNowRule
		{
			public override bool IsValid(DateTime value, DateTime now)
			{
				return value >= now;
			}

			public override ValidationIssue CreateIssue(ValidationContext context)
			{
				return context.Issue($"'{context.ValuePath}' should be later NOW.");
			}
		}
	}
}