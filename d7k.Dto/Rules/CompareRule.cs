using System;
using System.Collections.Generic;

namespace d7k.Dto
{
	public abstract class CompareRule : BaseValidationRule
	{
		public object Value { get; set; }

		public override ValidationResult Validate(ValidationContext context, ref object value)
		{
			if (value == null)
				return null;

			if (value is IComparable)
			{
				var tValue = Convert.ChangeType(Value, value.GetType());

				var resultMessage = Compare((IComparable)value, context.ValuePath, tValue);

				if (resultMessage != null)
					return new[] { context.Issue(this, $"{nameof(CompareRule)}.{GetType().Name}", $"'{context.ValuePath}' is not greater than {Value}.") }.ToResult();
			}

			return null;
		}

		protected abstract string Compare(IComparable sourceValue, string valuePath, object checkValue);

		public class Greater : CompareRule
		{
			protected override string Compare(IComparable sourceValue, string valuePath, object checkValue)
			{
				if (sourceValue.CompareTo(checkValue) <= 0)
					return $"'{valuePath}' is not greater than {Value}.";
				return null;
			}
		}

		public class Lesser : CompareRule
		{
			protected override string Compare(IComparable sourceValue, string valuePath, object checkValue)
			{
				if (sourceValue.CompareTo(checkValue) >= 0)
					return $"'{valuePath}' is not lesser than {Value}.";
				return null;
			}
		}

		public class NotGreater : CompareRule
		{
			protected override string Compare(IComparable sourceValue, string valuePath, object checkValue)
			{
				if (sourceValue.CompareTo(checkValue) > 0)
					return $"'{valuePath}' is greater than {Value}.";
				return null;
			}
		}

		public class NotLesser : CompareRule
		{
			protected override string Compare(IComparable sourceValue, string valuePath, object checkValue)
			{
				if (sourceValue.CompareTo(checkValue) < 0)
					return $"'{valuePath}' is lesser than {Value}.";
				return null;
			}
		}
	}
}
