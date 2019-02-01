namespace d7k.Filters
{
	public class ToLowerFilter : IStringFilter
	{
		public string Clean(string pn)
		{
			if (string.IsNullOrWhiteSpace(pn))
				return null;

			return pn.ToLower();
		}
	}
}
