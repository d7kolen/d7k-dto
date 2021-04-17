using System.Collections.Generic;
using System.Text;

namespace d7k.Dto
{
	class BasicNameFilter
	{
		public const string c_latin = "ABCDEFGHIJKLMONPQRSTUVWXYZ";
		public const string c_cirilic = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
		public const string c_numbers = "1234567890";
		public const string c_additionalFileNameAvailableChars = "~+=-_().";

		public readonly static string s_allLetter;
		public readonly static string s_whiteSpaces;

		static BasicNameFilter()
		{
			var lettersAccum = new StringBuilder();
			var whiteAccum = new StringBuilder();

			for (char t = char.MinValue; t < char.MaxValue; t++)
			{
				if (char.IsLetter(t))
					lettersAccum.Append(t);
				if (char.IsWhiteSpace(t))
					whiteAccum.Append(t);
			}

			s_allLetter = lettersAccum.ToString();
			s_whiteSpaces = whiteAccum.ToString();
		}

		HashSet<char> m_valid;
		bool m_toUpperResult;
		string m_toReplace;

		public BasicNameFilter(bool toUpperResult = true, string toReplace = "")
		{
			m_toUpperResult = toUpperResult;
			m_toReplace = toReplace;
			m_valid = new HashSet<char>();
			AddValidChars(c_latin).AddValidChars(c_numbers);
		}

		public BasicNameFilter Reset()
		{
			m_valid.Clear();
			return this;
		}

		public BasicNameFilter AddValidChars(string chars)
		{
			foreach (var t in chars)
				m_valid.Add(t);

			return this;
		}

		public string Clean(string pn)
		{
			if (string.IsNullOrWhiteSpace(pn))
				return null;

			var accum = new StringBuilder("");

			if (m_toUpperResult)
				CleanWithUpper(pn, accum);
			else
				Clean(pn, accum);

			if (accum.Length == 0)
				return null;

			return accum.ToString();
		}

		private void Clean(string pn, StringBuilder accum)
		{
			foreach (var t in pn)
			{
				var upT = char.ToUpper(t);
				if (m_valid.Contains(upT))
					accum.Append(t);
				else
					accum.Append(m_toReplace);
			}
		}

		private void CleanWithUpper(string pn, StringBuilder accum)
		{
			foreach (var t in pn)
			{
				var upT = char.ToUpper(t);
				if (m_valid.Contains(upT))
					accum.Append(upT);
				else
					accum.Append(m_toReplace);
			}
		}
	}
}
