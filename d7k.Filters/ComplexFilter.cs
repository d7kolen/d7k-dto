using System.Collections.Generic;

namespace d7k.Filters
{
	public class ComplexFilter : IStringFilter
	{
		List<IStringFilter> m_filters = new List<IStringFilter>();

		public ComplexFilter Add(IStringFilter filter)
		{
			m_filters.Add(filter);
			return this;
		}

		public string Clean(string pn)
		{
			foreach (var t in m_filters)
				pn = t.Clean(pn);

			return pn;
		}
	}
}
