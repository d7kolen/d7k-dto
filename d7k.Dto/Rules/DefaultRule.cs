using System;

namespace d7k.Dto
{
	public class DefaultRule : BaseValidationRule
	{
		public Func<object> Default { get; set; }

		public override ValidationResult Validate(ValidationContext context, ref object value)
		{
			if (NotEmptyRule.IsEmpty(value))
				value = Default();

			return null;
		}
	}
}
