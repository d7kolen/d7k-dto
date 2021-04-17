using System;
using System.Collections.Generic;
using System.Linq;

namespace d7k.Dto
{
	public class CheckRule<TValue> : BaseValidationRule
	{
		public Func<ValidationContext, TValue, IEnumerable<ValidationIssue>> ValidateFunc { get; set; }

		public override ValidationResult Validate(ValidationContext context, ref object value)
		{
			if (value == null || !(value is TValue))
				return null;

			var issues = ValidateFunc(context, (TValue)value);
			foreach (var t in issues ?? Enumerable.Empty<ValidationIssue>())
				if (t.Validator == null)
					t.Validator = this;

			return issues.ToResult();
		}
	}
}
