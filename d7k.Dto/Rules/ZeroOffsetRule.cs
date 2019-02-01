using System;

namespace d7k.Dto
{
	public class ZeroOffsetRule : BaseValidationRule
	{
		public override ValidationResult Validate(ValidationContext context, ref object value)
		{
			if (value == null)
				return null;

			var tCheckValue = (DateTimeOffset)value;
			if (tCheckValue.Offset != TimeSpan.Zero)
				return context.Issue(this, nameof(ZeroOffsetRule), $"'{context.ValuePath}' must have Zero Offset.").ToResult();

			return null;
		}
	}
}
