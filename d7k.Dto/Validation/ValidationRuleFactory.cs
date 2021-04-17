using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace d7k.Dto
{
	public class ValidationRuleFactory<TSource>
	{
		List<PathValidation<TSource>> m_validators = new List<PathValidation<TSource>>();
		Dictionary<Guid, Func<TSource, bool>> m_conditions = new Dictionary<Guid, Func<TSource, bool>>();

		public IValidationRepository Repository { get; private set; }
		public object OriginalValue { get; private set; }

		public ValidationRuleFactory(IValidationRepository repository, object originalValue)
		{
			Repository = repository;
			OriginalValue = originalValue;
		}

		public ValidationRuleFactory<T> CreateBySettings<T>()
		{
			return new ValidationRuleFactory<T>(Repository, OriginalValue);
		}

		public PathValidation<TSource, TProperty> RuleForIf<TProperty>(IPath<TSource> path, Func<TSource, bool> condition)
		{
			var indexer = Repository.GetIndexer(path);
			var tValidator = new PathValidation<TSource, TProperty>(indexer, this);

			m_validators.Add(tValidator);

			if (condition != null)
				m_conditions[tValidator.Id] = condition;

			return tValidator;
		}

		public ValidationRuleFactory<TSource> Clone()
		{
			var result = new ValidationRuleFactory<TSource>(Repository, OriginalValue);
			CopyTo(result);

			return result;
		}

		public ValidationRuleFactory<TSource> Clone(TSource newOriginalValue)
		{
			var result = new ValidationRuleFactory<TSource>(Repository, newOriginalValue);
			CopyTo(result);

			return result;
		}

		private void CopyTo(ValidationRuleFactory<TSource> accum)
		{
			foreach (var t in m_validators)
				accum.m_validators.Add(t.Clone());

			foreach (var t in m_conditions)
				accum.m_conditions[t.Key] = t.Value;
		}

		public ValidationRuleFactory<TSource> Append(ValidationRuleFactory<TSource> otherFactory)
		{
			otherFactory.CopyTo(this);
			return this;
		}

		public ValidationRuleFactory<TSource> Clear()
		{
			m_validators.Clear();
			m_conditions.Clear();
			return this;
		}

		public IEnumerable<PathValidation<TSource>> AllValidators(TSource source)
		{
			var accum = new Dictionary<string, PathValidation<TSource>>();

			foreach (var t in m_validators)
				if (!m_conditions.TryGetValue(t.Id, out Func<TSource, bool> check)
					||
					check(source))
				{
					if (!accum.TryGetValue(t.Path, out PathValidation<TSource> tPath))
						accum[t.Path] = tPath = t.Clone();
					else
						tPath.Validators.AddRange(t.Validators);
				}

			return accum.Values;
		}

		ValidationResult ValidateObject(PathValidation<TSource> path, ValidationContext context, object[] valueStor)
		{
			var result = new ValidationResult();
			var currentValue = valueStor[0];

			foreach (var tValidator in path.Validators)
				result.Load(tValidator.Validate(context, ref currentValue));

			valueStor[0] = currentValue;
			return result;
		}

		public ValidationResult ValidateObject(TSource obj, ValidationContext context)
		{
			var result = new ValidationResult();

			foreach (var tPathValidator in AllValidators(obj))
				foreach (var tPath in tPathValidator.Indexer.GetPathes(obj))
				{
					var value = tPath.GetValue();
					var valueStor = new[] { value };

					var tContext = context.SubPath(tPath.Path, value);

					var tResult = ValidateObject(tPathValidator, tContext, valueStor);
					result.Load(tResult);

					if (context.WithUpdate)
						tPath.SetValue(valueStor[0]);
					else
						result.DefferedUpdate(tPath, valueStor[0]);
				}

			return result;
		}
	}
}
