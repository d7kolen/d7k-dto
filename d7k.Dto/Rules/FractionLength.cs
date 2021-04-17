using System;

namespace d7k.Dto
{
	public class MantissaLength : BaseValidationRule
	{
		public int Length { get; set; }

		public override ValidationResult Validate(ValidationContext context, ref object value)
		{
			if (value == null)
				return null;
			
			if (value is decimal?)
			{
				var number = (decimal?)value;
				value = FixDecimal(number.Value);
				return null;
			}
			if (value is decimal)
			{
				value = FixDecimal((decimal)value);
				return null;
			}
			if (value is double?)
			{
				var number = (double?)value;
				value = FixDouble(number.Value);
				return null;
			}
			if (value is double)
			{
				value = FixDouble((double)value);
				return null;
			}
			if (value is float?)
			{
				var number = (float?)value;
				value = FixFloat(number.Value);
				return null;
			}
			if (value is float)
			{
				value = FixFloat((float)value);
				return null;
			}

			return null;
		}

		private decimal FixDecimal(decimal value)
		{
			return Math.Round(value, Length, MidpointRounding.AwayFromZero);
		}

		private double FixDouble(double value)
		{
			return Math.Round(value, Length, MidpointRounding.AwayFromZero);
		}

		private float FixFloat(float value)
		{
			return  (float)Math.Round(value, Length, MidpointRounding.AwayFromZero);
		}
	}
}
