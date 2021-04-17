using System;
using System.Linq.Expressions;

namespace d7k.Dto.Tests
{
	public class PathValueIndexerFactory
	{
		static ObjectFieldsPathFactory m_factory = new ObjectFieldsPathFactory();

		public static PathValueIndexer<TSource> Create<TSource, TProperty>(TSource example, Expression<Func<TSource, TProperty>> getter)
		{
			return PathValueIndexer<TSource>.Create<TProperty>(m_factory.GetPathItems(getter));
		}
	}
}
