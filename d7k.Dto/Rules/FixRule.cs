using System;

namespace d7k.Dto
{
	public class FixRule<TValue> : BaseValidationRule
	{
		public Func<TValue, TValue> FixFunc { get; set; }

		public override ValidationResult Validate(ValidationContext context, ref object value)
		{
			value = FixFunc((TValue)value);
			return null;
		}
	}
}
