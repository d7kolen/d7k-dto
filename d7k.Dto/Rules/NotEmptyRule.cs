using System;
using System.Collections.Generic;

namespace d7k.Dto
{
	public class NotEmptyRule : BaseValidationRule
	{
		public override ValidationResult Validate(ValidationContext context, ref object value)
		{
			if (IsEmpty(value))
				return EmptyIssue(context);
			return null;
		}

		public static bool IsEmpty(object value)
		{
			if (value == null)
				return true;

			if (value is string)
				return string.IsNullOrWhiteSpace((string)value);

			if (value is Array)
				return ((Array)value).Length == 0;

			if (value is System.Collections.IList)
				return ((System.Collections.IList)value).Count == 0;

			if (value.GetType().IsClass)
				return false;

			if (value is IComparable && !value.GetType().IsEnum)
			{
				var defaultValue = Activator.CreateInstance(value.GetType());
				if (((IComparable)value).CompareTo(defaultValue) == 0)
					return true;
			}

			return false;
		}

		private ValidationResult EmptyIssue(ValidationContext context)
		{
			return context.Issue(this, nameof(NotEmptyRule), $"'{context.ValuePath}' cannot be empty.").ToResult();
		}
	}
}
