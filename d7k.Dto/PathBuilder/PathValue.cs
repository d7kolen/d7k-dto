using System;

namespace d7k.Dto
{
	public class PathValue<TSource>
	{
		TSource m_source;
		object[] m_indexes;
		Func<TSource, object[], object> m_get;
		Action<TSource, object[], object> m_set;
		Func<TSource, object[], bool> m_availability;

		public string Path { get; private set; }

		public PathValue(TSource source, object[] indexes, Func<TSource, object[], object> get, Action<TSource, object[], object> set, Func<TSource, object[], bool> availability, string path)
		{
			m_source = source;
			m_indexes = indexes;
			m_get = get;
			m_set = set;
			m_availability = availability;
			Path = path;
		}

		public object GetValue()
		{
			return m_get(m_source, m_indexes);
		}

		public void SetValue(object value)
		{
			m_set(m_source, m_indexes, value);
		}

		public bool IsAvailable()
		{
			return m_availability(m_source, m_indexes);
		}
	}
}
