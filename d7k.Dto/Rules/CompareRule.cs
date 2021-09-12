using System;

namespace d7k.Dto
{
	public abstract class CompareRule : BaseValidationRule
	{
		public bool FixToValue { get; set; }
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
					if (FixToValue)
						value = Value;
					else
					{
						var description = new BasicDescription() { Path = context.ValuePath };

						var issue = context.Issue(
							this,
							$"{nameof(CompareRule)}.{GetType().Name}",
							$"'{context.ValuePath}' is not greater than {Value}.",
							description);

						return new[] { issue }.ToResult();
					}
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
