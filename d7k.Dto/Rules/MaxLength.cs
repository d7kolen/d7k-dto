using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace d7k.Dto
{
	public class MaxLength : BaseValidationRule
	{
		public int Length { get; set; }

		public override ValidationResult Validate(ValidationContext context, ref object value)
		{
			if (value == null)
				return null;

			if (value is string)
			{
				var str = (string)value;
				if (str.Length > Length)
					value = str.Substring(0, Length);
				return null;
			}
			if (value is Array)
			{
				var source = (Array)value;
				if (source.Length <= Length)
					return null;

				var array = (Array)Activator.CreateInstance(value.GetType(), new object[] { Length });

				Array.Copy(source, array, Length);

				value = array;
				return null;
			}

			if (value is IList)
			{
				var source = (IList)value;
				if (source.Count <= Length)
					return null;

				var valueAccum = (IList)Activator.CreateInstance(value.GetType());

				for (int i = 0; i < Length; i++)
					valueAccum.Add(source[i]);

				value = valueAccum;
				return null;
			}
			return null;
		}
	}
}
