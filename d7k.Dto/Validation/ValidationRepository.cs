using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace d7k.Dto
{
	public class ValidationRepository
	{
		ConcurrentDictionary<string, object> m_indexerStorage = new ConcurrentDictionary<string, object>();

		const string c_separator = "#&%$*$%&#";

		public PathValueIndexer<TSource> GetIndexer<TSource, TProperty>(Expression<Func<TSource, TProperty>> expression, string name)
		{
			var storageKey = $"{typeof(TSource).FullName}{c_separator}{typeof(TProperty).FullName}{c_separator}{name}";

			var tIndexer = m_indexerStorage.GetOrAdd(storageKey, x => PathValueIndexer<TSource>.Create(expression));
			return (PathValueIndexer<TSource>)tIndexer;
		}

		public ValidationResult Validate<TSource>(TSource source, Action<ValidationRuleFactory<TSource>> init)
		{
			var factory = Create(source, init);

			return factory.ValidateObject(source, new ValidationContext());
		}

		public TSource Fix<TSource>(TSource source, Action<ValidationRuleFactory<TSource>> init)
		{
			var factory = Create(source, init);
			return factory.Fix(source);
		}

		public TSource FixValue<TSource>(TSource value, string valueName, Action<PathValidation<object, TSource>> init)
		{
			var context = new ValidationContext(valueName, value);

			return FixValue(value, context, init);
		}

		private TSource FixValue<TSource>(TSource value, ValidationContext context, Action<PathValidation<object, TSource>> init)
		{
			var factory = new ValidationRuleFactory<object>(this, value);

			var path = new PathValidation<object, TSource>(null, factory);
			init(path);

			var valueStor = new[] { value };
			object objValue = value;

			var result = new ValidationResult();

			foreach (var tValidator in path.Validators)
				result.Load(tValidator.Validate(context, ref objValue));

			result.ThrowIssues();

			context.WithUpdate = true;

			foreach (var tValidator in path.Validators)
				tValidator.Validate(context, ref objValue);

			return (TSource)objValue;
		}

		#region Memory leak

		//public TSource FixValue<TSource>(Expression<Func<TSource>> getValue, Action<PathValidation<object, TSource>> init)
		//{
		//	var name = NameOf.nameof(getValue);
		//	var value = getValue.Compile().Invoke();
		//
		//	return FixValue(value, name, init);
		//}

		#endregion Memory leak

		public ValidationRuleFactory<T> Create<T>(T value, Action<ValidationRuleFactory<T>> init = null)
		{
			var result = new ValidationRuleFactory<T>(this, value);
			init?.Invoke(result);

			return result;
		}
	}
}
