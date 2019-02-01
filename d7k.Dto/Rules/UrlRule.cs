using System;

namespace d7k.Dto
{
	public class UrlRule : BaseValidationRule
	{
		public override ValidationResult Validate(ValidationContext context, ref object value)
		{
			if (value == null)
				return null;

			if (value is string)
			{
				try
				{
					var addr = new Uri((string)value);
				}
				catch
				{
					return context.Issue(this, nameof(EmailRule), $"'{context.ValuePath}' has invalid URL format.").ToResult();
				}
			}

			return null;
		}
	}
}
