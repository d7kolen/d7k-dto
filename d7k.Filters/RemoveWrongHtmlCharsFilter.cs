using System.Text.RegularExpressions;

namespace d7k.Filters
{
	public class RemoveWrongHtmlCharsFilter : IStringFilter
	{
		public string Clean(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return null;

			const string pattern = @"[^\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]";
			return Regex.Replace(text, pattern, "");
		}
	}
}
