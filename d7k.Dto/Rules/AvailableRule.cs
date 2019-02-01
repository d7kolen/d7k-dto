using System.Collections.Generic;

namespace d7k.Dto
{
	public class AvailableRule : BaseValidationRule
	{
		public List<object> Values { get; set; } = new List<object>();

		public override ValidationResult Validate(ValidationContext context, ref object value)
		{
			if (NotEmptyRule.IsEmpty(value))
				return null;

			foreach (var t in Values)
				if (value.Equals(t))
					return null;

			return context.Issue(this, nameof(AvailableRule), $"'{context.ValuePath}' doesn't have available values for [{value}].").ToResult();
		}
	}
}
