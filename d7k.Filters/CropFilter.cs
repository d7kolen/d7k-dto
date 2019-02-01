namespace d7k.Filters
{
	public class CropFilter : IStringFilter
	{
		int m_maxSize;

		public CropFilter(int maxSize)
		{
			m_maxSize = maxSize;
		}

		public string Clean(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return null;

			if (text.Length > m_maxSize)
				return text;

			return text.Substring(0, m_maxSize);
		}
	}
}
