using System;

namespace d7k.Dto
{
	public class ValidationContext
	{
		static readonly StringComparer s_pathComparer = StringComparer.Ordinal;

		public string ValuePath { get; set; }
		public object OriginValue { get; set; }
		public bool WithUpdate { get; set; }

		public ValidationContext(string valuePath, object originValue)
		{
			ValuePath = valuePath;
			OriginValue = originValue;
		}

		public ValidationContext()
			: this("", null)
		{
		}

		public ValidationIssue Issue(BaseValidationRule validator, string code, string message, IIssueDescription description)
		{
			return new ValidationIssue(validator, ValuePath, OriginValue, code, message, description);
		}

		public ValidationIssue Issue(BaseValidationRule validator, string code, string message)
		{
			return new ValidationIssue(validator, ValuePath, OriginValue, code, message, null);
		}

		public ValidationIssue Issue(string code, string message)
		{
			return new ValidationIssue(null, ValuePath, OriginValue, code, message, null);
		}

		public ValidationIssue Issue(string message)
		{
			return new ValidationIssue(null, ValuePath, OriginValue, null, message, null);
		}

		public ValidationContext SubPath(string path, object value)
		{
			return new ValidationContext(GetSubPathValue(path), OriginValue)
			{
				WithUpdate = WithUpdate
			};
		}

		public string GetSubPathValue(string path)
		{
			return ValuePath + path;
		}
	}
}
