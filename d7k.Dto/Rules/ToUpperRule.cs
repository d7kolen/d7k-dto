
namespace d7k.Dto
{
	public class ToUpperRule : BaseValidationRule
	{
		public override ValidationResult Validate(ValidationContext context, ref object value)
		{
			if (value == null)
				return null;

			if (value is string)
				value = ((string)value).ToUpper();

			return null;
		}
	}
}
