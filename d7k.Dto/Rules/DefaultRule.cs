using System;

namespace d7k.Dto
{
	public class DefaultRule : BaseValidationRule
	{
		Type m_valueType;
		public Func<object> Default { get; set; }

		public DefaultRule(Type valueType)
		{
			m_valueType = valueType;
		}

		public override ValidationResult Validate(ValidationContext context, ref object value)
		{
			if (NotEmptyRule.IsEmpty(value, m_valueType))
				value = Default();

			return null;
		}
	}
}
