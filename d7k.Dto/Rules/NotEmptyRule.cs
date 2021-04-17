using System;
using System.Collections.Generic;

namespace d7k.Dto
{
	public class NotEmptyRule : BaseValidationRule
	{
		Type m_propertyType;

		#region s_availableZeroTypes

		static HashSet<Type> s_availableZeroTypes = new HashSet<Type>(
			new[] {
				typeof(sbyte), typeof(byte),
				typeof(Int16), typeof(UInt16),
				typeof(Int32), typeof(UInt32),
				typeof(Int64), typeof(UInt64),
				typeof(Single), typeof(double), typeof(decimal),
				typeof(bool)
			});

		#endregion

		public NotEmptyRule(Type propertyType)
		{
			m_propertyType = propertyType;
		}

		public override ValidationResult Validate(ValidationContext context, ref object value)
		{
			if (IsEmpty(value, m_propertyType))
				return EmptyIssue(context);
			return null;
		}

		public static bool IsEmpty(object value, Type propertyType = null)
		{
			if (value == null)
				return true;

			if (value is string)
				return string.IsNullOrWhiteSpace((string)value);

			if (value is Array)
				return ((Array)value).Length == 0;

			if (value is System.Collections.IList)
				return ((System.Collections.IList)value).Count == 0;

			if (value.GetType().IsClass)
				return false;

			var valueType = (propertyType == null || propertyType == typeof(object)) ? value.GetType() : propertyType;

			//Zero is not Empty
			if (s_availableZeroTypes.Contains(valueType))
			{
				var tZero = Convert.ChangeType(0, valueType);
				if (value.Equals(tZero))
					return false;
			}

			if (value is IComparable && !valueType.IsEnum)
			{
				var defaultValue = Activator.CreateInstance(valueType);
				if (((IComparable)value).CompareTo(defaultValue) == 0)
					return true;
			}

			return false;
		}

		public static object DefaultValue(Type type)
		{
			if (type == typeof(string))
				return "";

			if (type.IsArray)
				return Activator.CreateInstance(type, 0);

			if (type.IsInterface)
				return DtoFactory.Dto(type);

			//Nullable available 0 default
			if (type.Name == typeof(int?).Name)
			{
				var argType = type.GenericTypeArguments[0];
				if (s_availableZeroTypes.Contains(argType))
					return Convert.ChangeType(0, argType);
			}

			return Activator.CreateInstance(type);
		}

		private ValidationResult EmptyIssue(ValidationContext context)
		{
			return context.Issue(this, nameof(NotEmptyRule), $"'{context.ValuePath}' cannot be empty.").ToResult();
		}
	}
}
