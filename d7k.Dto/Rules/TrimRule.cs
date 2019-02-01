using System.Collections;
using System.Linq;

namespace d7k.Dto
{
	public class TrimRule : BaseValidationRule
	{
		public override ValidationResult Validate(ValidationContext context, ref object value)
		{
			if (value == null)
				return null;

			if (value is string)
			{
				var strValue = (string)value;

				strValue = strValue.Trim();
				if (strValue.Length == 0)
					strValue = null;

				value = strValue;
				return null;
			}

			if (value is IEnumerable)
			{
				var enValue = (IEnumerable)value;
				if (!enValue.Cast<object>().Any())
					value = null;

				return null;
			}

			return null;
		}
	}
}
