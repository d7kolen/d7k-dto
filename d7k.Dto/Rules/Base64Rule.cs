using System;

namespace d7k.Dto
{
	public class Base64Rule : BaseValidationRule
	{
		public override ValidationResult Validate(ValidationContext context, ref object value)
		{
			if (value == null || !(value is string))
				return null;

			try
			{
				Convert.FromBase64String((string)value);
			}
			catch (FormatException)
			{
				return new[] { context.Issue(this, $"{nameof(Base64Rule)}.{GetType().Name}", $"'{context.ValuePath}' has wrong base64 format value '{value}'.") }.ToResult();
			}

			return null;
		}
	}
}
