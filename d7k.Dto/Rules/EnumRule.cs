using System;

namespace d7k.Dto
{
	public class EnumRule : BaseValidationRule
	{
		public override ValidationResult Validate(ValidationContext context, ref object value)
		{
			if (value == null)
				return null;

			if (value.GetType().IsEnum)
			{
				if (!Enum.IsDefined(value.GetType(), value))
					return context.Issue(this, nameof(EmailRule), $"'{context.ValuePath}' has invalid enumeration [{value.GetType().Name}] value [{value}].").ToResult();
			}

			return null;
		}
	}
}
