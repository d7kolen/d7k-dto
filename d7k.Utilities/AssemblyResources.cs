using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace d7k.Utilities
{
	public class AssemblyResources
	{
		string m_prefix;
		Dictionary<string, Stream> m_resources;
		Assembly m_assembly;

		public AssemblyResources(Assembly assembly, string prefix)
		{
			m_assembly = assembly;
			m_prefix = prefix?.Trim()?.TrimEnd('.');
			Init();
		}

		public IEnumerable<string> Keys { get { return m_resources.Keys; } }

		public Stream this[string key]
		{
			get
			{
				Stream res;
				if (m_resources.TryGetValue(key, out res))
					return res;

				if (m_resources.TryGetValue(m_prefix + "." + key, out res))
					return res;

				throw new FileNotFoundException("Resource file was not found", key);
			}
		}

		void Init()
		{
			m_resources = new Dictionary<string, Stream>(StringComparer.InvariantCultureIgnoreCase);

			foreach (var t in m_assembly.GetManifestResourceNames())
				m_resources[t] = m_assembly.GetManifestResourceStream(t);
		}
	}
}
