using System.Collections;
using System.Linq;

namespace d7k.Dto
{
	public class LengthBetweenRule : BaseValidationRule
	{
		public int? MinLength { get; set; }
		public int? MaxLength { get; set; }

		public override ValidationResult Validate(ValidationContext context, ref object value)
		{
			if (value == null)
				return null;

			if (value is string)
			{
				var sValue = (string)value;
				if (MinLength != null && sValue.Length < MinLength)
					return context.Issue(this, nameof(LengthBetweenRule), $"'{context.ValuePath}' length lesser then allowable '{MinLength}'.").ToResult();
				else if (MaxLength != null && sValue.Length > MaxLength)
					return context.Issue(this, nameof(LengthBetweenRule), $"'{context.ValuePath}' length greater then allowable '{MaxLength}'.").ToResult();
				return null;
			}

			if (value is IEnumerable)
			{
				var arrValue = (IEnumerable)value;
				if (MinLength != null && arrValue.Cast<object>().Count() < MinLength)
					return context.Issue(this, nameof(LengthBetweenRule), $"'{context.ValuePath}' length lesser then allowable '{MinLength}'.").ToResult();
				else if (MaxLength != null && arrValue.Cast<object>().Count() > MaxLength)
					return context.Issue(this, nameof(LengthBetweenRule), $"'{context.ValuePath}' length greater then allowable '{MaxLength}'.").ToResult();
				return null;
			}

			return null;
		}
	}
}
