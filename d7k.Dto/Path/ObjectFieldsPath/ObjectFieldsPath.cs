using d7k.Dto.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace d7k.Dto
{
	public class ObjectFieldsPath<TSource> : IPath<TSource>
	{
		List<PathItem> m_items { get; }
		int m_indexCount;

		Func<TSource, object[], object> m_get;
		Action<TSource, object[], object> m_set;
		Func<TSource, object[], bool> m_available;
		Func<TSource, object[], object[]> m_indexes;

		public ObjectFieldsPath(List<PathItem> path)
		{
			m_items = path;
		}

		public void PrepareForIndexing()
		{
			m_indexCount = m_items.Where(x => x.Array != null).Count();

			m_get = GetFunctionBuilder<TSource>.GetFunction(m_items);
			m_set = new SetFunctionBuilder<TSource>(m_items).Build();
			m_available = new ExistFunctionBuilder<TSource>(m_items).Build();
			m_indexes = new IndexesFunctionBuilder<TSource>(m_items).Build();
		}

		public object Get(TSource source, object[] index)
		{
			return m_get(source, index);
		}

		public void Set(TSource source, object[] index, object value)
		{
			m_set(source, index, value);
		}

		public string IndexedName(object[] index)
		{
			int indexNum = 0;

			var pathAccum = new StringBuilder("");
			foreach (var t in m_items)
				if (t.Property != null)
					pathAccum.Append(".").Append(t.Property.Name);
				else
				{
					var tIndex = index[indexNum];
					if (tIndex is string)
						tIndex = $"'{tIndex}'";

					pathAccum.Append("[").Append(tIndex).Append("]");
					indexNum++;
				}

			return pathAccum.ToString();
		}

		public string PathName()
		{
			var pathAccum = new StringBuilder("");
			foreach (var t in m_items)
				if (t.Property != null)
					pathAccum.Append(".").Append(t.Property.Name);
				else
					pathAccum.Append("[]");

			return pathAccum.ToString();
		}

		public IEnumerable<object[]> GetAllIndexes(TSource source)
		{
			foreach (var t in GetIndexesForPrefix(source, new object[0]))
				if (m_available(source, t))
					yield return t;
		}

		IEnumerable<object[]> GetIndexesForPrefix(TSource source, object[] previousIndex)
		{
			if (previousIndex.Length == m_indexCount)
			{
				yield return previousIndex;
				yield break;
			}

			var lastIndexes = m_indexes(source, previousIndex);
			if (lastIndexes == null)
				yield break;

			foreach (var tIt in lastIndexes)
			{
				var newIndexes = previousIndex.AddItem(tIt).ToArray();

				foreach (var t in GetIndexesForPrefix(source, newIndexes))
					yield return t;
			}
		}
	}
}
