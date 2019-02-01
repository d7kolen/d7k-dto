namespace d7k.Filters
{
	public class TrimFilter : IStringFilter
	{
		char[] m_chars;
		public TrimFilter(string chars = "")
		{
			m_chars = chars.ToCharArray();
		}

		public string Clean(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return null;

			if (m_chars.Length == 0)
				return text.Trim();
			else
				return text.Trim(m_chars);
		}
	}
}
