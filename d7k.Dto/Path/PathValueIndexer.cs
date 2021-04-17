using System.Collections.Generic;

namespace d7k.Dto
{
	public class PathValueIndexer<TSource>
	{
		IPath<TSource> m_path;
		public string PathName { get; private set; }

		public static ObjectFieldsPathFactory m_factory = new ObjectFieldsPathFactory();

		/// <summary>
		/// Please use First() for value indexing
		/// </summary>
		public static PathValueIndexer<TSource> Create<TProperty>(IPath<TSource> path)
		{
			var result = new PathValueIndexer<TSource>();

			path.PrepareForIndexing();

			result.m_path = path;
			result.PathName = path.PathName();

			return result;
		}

		private PathValueIndexer() { }

		public IEnumerable<PathValue<TSource>> GetPathes(TSource source)
		{
			var indexes = m_path.GetAllIndexes(source);
			foreach (var t in indexes)
				yield return new PathValue<TSource>(source, t, m_path.Get, m_path.Set, m_path.IndexedName(t));
		}
	}
}
