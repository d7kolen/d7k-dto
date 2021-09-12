using System;
using System.Collections.Generic;
using System.Linq;

namespace d7k.Dto
{
	public class ValidationIssue
	{
		public BaseValidationRule Validator { get; set; }
		public string ValuePath { get; set; }
		public object CheckValue { get; set; }
		public string Code { get; set; }
		public string Message { get; set; }
		public IIssueDescription Description { get; set; }

		public ValidationIssue(BaseValidationRule validator, string valuePath, object checkValue, string code, string message, IIssueDescription description)
		{
			Validator = validator;
			ValuePath = valuePath;
			CheckValue = checkValue;
			Code = code;
			Message = message;
			Description = description;
		}
	}

	public class ValidationResult
	{
		public List<ValidationIssue> Issues { get; } = new List<ValidationIssue>();

		List<Action> m_updates = new List<Action>();

		public virtual ValidationResult ThrowIssues()
		{
			if (Issues.Any())
				throw new ValidationException(Issues);

			return this;
		}

		public ValidationResult DefferedUpdate<TSource>(PathValue<TSource> path, object value)
		{
			m_updates.Add(() => path.SetValue(value));
			return this;
		}

		public ValidationResult Load(ValidationResult otherResult)
		{
			if (otherResult == null)
				return this;

			Issues.AddRange(otherResult.Issues);
			m_updates.AddRange(otherResult.m_updates);

			return this;
		}

		/// <summary>
		/// The update is not equivalent of Fix methods. Fix method will make to update at once, but the method will aggregate all updates and after that will do them.
		/// So in case when updater want to use already changed data (e.g. SkipEmpty) you need to use Fix functions
		/// </summary>
		public void Update()
		{
			m_updates.ForEach(x => x());
		}
	}

	static class ValidationResultHelper
	{
		public static ValidationResult ToResult(this IEnumerable<ValidationIssue> issues)
		{
			var result = new ValidationResult();
			if (issues != null)
				result.Issues.AddRange(issues);
			return result;
		}

		public static ValidationResult ToResult(this ValidationIssue issues)
		{
			var result = new ValidationResult();
			result.Issues.Add(issues);
			return result;
		}
	}

	public class ValidationException : Exception
	{
		public List<ValidationIssue> Issues { get; set; }

		public ValidationException(string message)
			: base(message)
		{
		}

		public ValidationException(List<ValidationIssue> issues)
			: base(string.Join(Environment.NewLine, issues.Select(x => x.Message)))
		{
			Issues = issues;
		}
	}
}
