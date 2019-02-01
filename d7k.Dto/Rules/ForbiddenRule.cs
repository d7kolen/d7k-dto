using System.Collections.Generic;

namespace d7k.Dto
{
	public class ForbiddenRule : BaseValidationRule
	{
		public List<object> Items { get; set; } = new List<object>();

		public override ValidationResult Validate(ValidationContext context, ref object value)
		{
			if (NotEmptyRule.IsEmpty(value))
				return null;

			foreach (var t in Items)
				if (value.Equals(t))
					return context.Issue(this, nameof(ForbiddenRule), $"'{context.ValuePath}' has forbidden [{value.GetType().Name}] value [{value}].").ToResult();

			return null;
		}
	}
}
