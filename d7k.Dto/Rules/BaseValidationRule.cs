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

	public interface IIssueDescription
	{
		string Path { get; set; }
	}

	public class BasicDescription : IIssueDescription
	{
		public string Path { get; set; }
	}
}
