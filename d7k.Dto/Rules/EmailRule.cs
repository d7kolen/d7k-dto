
namespace d7k.Dto
{
	public class EmailRule : BaseValidationRule
	{
		public override ValidationResult Validate(ValidationContext context, ref object value)
		{
			if (value == null)
				return null;

			if (value is string)
			{
				try
				{
					var addr = new System.Net.Mail.MailAddress((string)value);
					if (addr.Address == (string)value)
						return null;
				}
				catch
				{
				}

				return context.Issue(this, nameof(EmailRule), $"'{context.ValuePath}' has invalid email format.").ToResult();
			}

			return null;
		}
	}
}
