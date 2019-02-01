namespace d7k.Dto
{
	public class BaseValidationRule
	{
		public string RuleName { get; set; }

		protected BaseValidationRule()
		{
			RuleName = GetType().FullName;
		}

		public virtual ValidationResult Validate(ValidationContext context, ref object value)
		{
			return null;
		}
	}
}
