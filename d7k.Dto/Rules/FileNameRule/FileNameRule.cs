namespace d7k.Dto
{
	public class FileNameRule : BaseValidationRule
	{
		public bool LatinOnly { get; set; }
		object m_sync = new object();
		BasicNameFilter m_filter;

		public override ValidationResult Validate(ValidationContext context, ref object value)
		{
			if (m_filter == null)
				lock (m_sync)
					if (m_filter == null)
						m_filter = CreateFilter();

			if (value == null)
				return null;

			if (value is string)
				value = m_filter.Clean((string)value);

			return null;
		}

		private BasicNameFilter CreateFilter()
		{
			var filter = new BasicNameFilter(false, "_").Reset()
				.AddValidChars(BasicNameFilter.c_numbers)
				.AddValidChars(BasicNameFilter.c_additionalFileNameAvailableChars);

			if (LatinOnly)
				filter.AddValidChars(BasicNameFilter.c_latin);
			else
				filter.AddValidChars(BasicNameFilter.s_allLetter);

			return filter;
		}
	}
}
