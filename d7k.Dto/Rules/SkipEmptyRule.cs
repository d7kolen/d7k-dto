using System;
using System.Collections.Generic;
using System.Linq;

namespace d7k.Dto
{
	public class SkipEmptyRule : BaseValidationRule
	{
		public override ValidationResult Validate(ValidationContext context, ref object value)
		{
			if (value == null || !(value is System.Collections.IEnumerable))
				return base.Validate(context, ref value);

			var tValue = (value as System.Collections.IEnumerable).Cast<object>().ToList();

			var accum = new List<object>();
			foreach (var t in tValue)
				if (!NotEmptyRule.IsEmpty(t))
					accum.Add(t);

			if (value is Array)
			{
				var valueAccum = (Array)Activator.CreateInstance(value.GetType(), new object[] { accum.Count });
				for (int i = 0; i < accum.Count; i++)
					valueAccum.SetValue(accum[i], i);

				value = valueAccum;
				return null;
			}
			else if (value is System.Collections.IList)
			{
				var valueAccum = (System.Collections.IList)Activator.CreateInstance(value.GetType());
				foreach (var t in accum)
					valueAccum.Add(t);

				value = valueAccum;
				return null;
			}

			throw new NotImplementedException();
		}
	}
}
