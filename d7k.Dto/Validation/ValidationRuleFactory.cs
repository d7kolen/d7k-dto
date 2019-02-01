using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace d7k.Dto
{
	public class ValidationRuleFactory<TSource>
	{
		Dictionary<string, PathValidation<TSource>> m_validators = new Dictionary<string, PathValidation<TSource>>();
		public IEnumerable<PathValidation<TSource>> AllValidators { get { return m_validators.Values; } }

		public ValidationRepository Repository { get; private set; }
		public object OriginalValue { get; private set; }

		public ValidationRuleFactory(ValidationRepository repository, object originalValue)
		{
			Repository = repository;
			OriginalValue = originalValue;
		}

		public ValidationRuleFactory<T> CreateBySettings<T>()
		{
			return new ValidationRuleFactory<T>(Repository, OriginalValue);
		}

		public PathValidation<TSource, TProperty> RuleFor<TProperty>(Expression<Func<TSource, TProperty>> expression)
		{
			var expressionPath = PathValueIndexer<TSource>.GetPathName(expression);

			if (!m_validators.ContainsKey(expressionPath))
			{
				var indexer = Repository.GetIndexer(expression, expressionPath);
				m_validators[expressionPath] = new PathValidation<TSource, TProperty>(indexer, this);
			}

			return (PathValidation<TSource, TProperty>)m_validators[expressionPath];
		}

		public ValidationRuleFactory<TSource> Clone()
		{
			var result = new ValidationRuleFactory<TSource>(Repository, OriginalValue);

			CopyValidators(result);

			return result;
		}

		public ValidationRuleFactory<TSource> Clone(TSource newOriginalValue)
		{
			var result = new ValidationRuleFactory<TSource>(Repository, newOriginalValue);

			CopyValidators(result);

			return result;
		}

		private void CopyValidators(ValidationRuleFactory<TSource> factory)
		{
			foreach (var t in m_validators)
				factory.m_validators[t.Key] = t.Value.Clone();
		}

		public ValidationRuleFactory<TSource> Append(ValidationRuleFactory<TSource> otherFactory)
		{
			var cloneOther = otherFactory.Clone();

			foreach (var tOther in cloneOther.m_validators)
				if (!m_validators.ContainsKey(tOther.Key))
					m_validators[tOther.Key] = tOther.Value;
				else
					m_validators[tOther.Key].Validators.AddRange(tOther.Value.Validators);

			return this;
		}

		public ValidationRuleFactory<TSource> Clear()
		{
			m_validators.Clear();
			return this;
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
			var allPathValidators = AllValidators.OrderBy(x => x.Path).Reverse().ToList();

			foreach (var tPathValidator in AllValidators)
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
