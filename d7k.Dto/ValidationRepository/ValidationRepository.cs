using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace d7k.Dto
{
	public class ValidationRepository : IValidationRepository
	{
		ConcurrentDictionary<string, object> m_indexerStorage = new ConcurrentDictionary<string, object>();

		const string c_separator = "#&%$*$%&#";

		public PathValueIndexer<TSource> GetIndexer<TSource>(IPath<TSource> path)
		{
			var pathName = path.PathName();

			var storageKey = $"{typeof(TSource).FullName}{c_separator}{pathName}";

			var tIndexer = m_indexerStorage.GetOrAdd(storageKey, x => PathValueIndexer<TSource>.Create<TSource>(path));
			return (PathValueIndexer<TSource>)tIndexer;
		}

		public ValidationRuleFactory<T> Create<T>(T value, Action<ValidationRuleFactory<T>> init = null)
		{
			var result = new ValidationRuleFactory<T>(this, value);
			init?.Invoke(result);

			return result;
		}

		Dictionary<Type, object> m_extensions = new Dictionary<Type, object>();

		public object Extension(Type type)
		{
			if (m_extensions.TryGetValue(type, out object extension))
				return extension;
			return this;
		}

		public ValidationRepository SetExtension(Type type, object extension)
		{
			m_extensions[type] = extension;
			return this;
		}
	}
}
