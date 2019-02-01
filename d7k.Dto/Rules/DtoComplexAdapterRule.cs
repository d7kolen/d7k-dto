
namespace d7k.Dto
{
	public interface IComplexAdapterRule
	{
		void Init(object factory, DtoComplex complex);
	}

	public class DtoComplexAdapterRule<T> : BaseValidationRule, IComplexAdapterRule
	{
		public ValidationRuleFactory<T> Factory { get; set; }
		public DtoComplex Complex { get; set; }

		public DtoComplexAdapterRule()
		{
		}

		void IComplexAdapterRule.Init(object factory, DtoComplex complex)
		{
			Factory = (ValidationRuleFactory<T>)factory;
			Complex = complex;
		}

		public override ValidationResult Validate(ValidationContext context, ref object value)
		{
			if (value == null)
				return null;

			return Factory.ValidateObject(Complex.Cast<T>(value), context);
		}
	}
}
