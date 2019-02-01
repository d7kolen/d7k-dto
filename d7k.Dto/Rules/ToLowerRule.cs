
namespace d7k.Dto
{
	public class ToLowerRule : BaseValidationRule
	{
		public override ValidationResult Validate(ValidationContext context, ref object value)
		{
			if (value == null)
				return null;

			if (value is string)
				value = ((string)value).ToLower();

			return null;
		}
	}
}
