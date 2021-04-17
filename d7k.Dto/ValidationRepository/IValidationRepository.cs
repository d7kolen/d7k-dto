using System;

namespace d7k.Dto
{
	public interface IValidationRepository
	{
		PathValueIndexer<TSource> GetIndexer<TSource>(IPath<TSource> path);
		ValidationRuleFactory<T> Create<T>(T value, Action<ValidationRuleFactory<T>> init = null);

		object Extension(Type type);
	}
}
