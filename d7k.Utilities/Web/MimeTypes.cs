using d7k.Utilities.Monads;
using d7k.Utilities.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace d7k.Utilities
{
	public class MimeTypes
	{
		Dictionary<string, string> m_types;
		Dictionary<string, string> m_extensions;

		public MimeTypes(IEnumerable<MimeTypeItem> mimeTypes)
		{
			m_types = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
			m_extensions = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

			foreach (var t in mimeTypes)
			{
				if (!m_types.ContainsKey(t.Extension) || t.Best != null)
					m_types[t.Extension] = t.MimeType;

				if (!m_extensions.ContainsKey(t.MimeType) || t.Best != null)
					m_extensions[t.MimeType] = t.Extension;
			}
		}

		public static MimeTypes Load()
		{
			var el = XElement.Parse(Resources.MimeTypes);

			var types = from x in el.Elements("type")
						select new MimeTypeItem()
						{
							Extension = x.Attribute("extension").Value,
							MimeType = x.Attribute("mimetype").Value,
							Best = x.Attributes("best").FirstOrDefault()?.Value
						};

			return new MimeTypes(types);
		}

		[Obsolete("Use TypeByExtension and ExtensionByType instead")]
		public string Get(string extension)
		{
			string res;
			return m_types.TryGetValue(extension, out res) ? res : "binary/octet-stream";
		}

		public string TypeByExtension(string extension)
		{
			string val;
			return m_types.TryGetValue(extension, out val) ? val : "binary/octet-stream";
		}

		public string ExtensionByType(string mimeType)
		{
			string val;
			return m_extensions.TryGetValue(mimeType, out val) ? val : ".bak";
		}
	}

	public class MimeTypeItem
	{
		public string Extension { get; set; }
		public string MimeType { get; set; }
		public string Best { get; set; }
	}
}