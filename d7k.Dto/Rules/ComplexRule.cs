
namespace d7k.Dto
{
	public class ComplexRule<T> : BaseValidationRule
	{
		public ValidationRuleFactory<T> Factory { get; set; }

		public override ValidationResult Validate(ValidationContext context, ref object value)
		{
			if (value == null || !(value is T))
				return null;

			return Factory.ValidateObject((T)value, context);
		}
	}
}
