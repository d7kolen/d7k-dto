using System;
using System.IO;

namespace d7k.Utilities
{
	public class TempFolder : IDisposable
	{
		private DisposeManager m_disposer;

		public DirectoryInfo Directory { get; private set; }

		public TempFolder(DirectoryInfo baseDir, TimeSpan dirtyTempTime, bool stupidTest = true)
		{
			if (stupidTest && baseDir.Name.IndexOf("temp", StringComparison.InvariantCultureIgnoreCase) < 0)
				throw new FormatException("Temp directory has not \"temp\" word in the part of name. You can lose an improtant data as I. Could you please reconfigurate a system or disable \"stupid test\"");

			Directory = baseDir.SubDir(Guid.NewGuid().ToString("N"));
			Directory.Create();

			m_disposer = new DisposeManager();
			m_disposer.OnDispose += () =>
			{
				try
				{
					Directory.Delete(true);
				}
				catch (IOException)
				{
				}

				var date = DateTime.UtcNow - dirtyTempTime;

				DirectoryInfo[] dirList;

				try
				{
					dirList = Directory.Parent.GetDirectories();
				}
				catch (IOException)
				{
					return;
				}


				foreach (var t in dirList)
					try
					{
						if (t.CreationTimeUtc < date)
							t.Delete(true);
					}
					catch (IOException)
					{
					}

			};
		}

		public void Dispose()
		{
			m_disposer.Dispose();
		}
	}

	public class TempFolderFactory
	{
		private DirectoryInfo m_baseDir;
		private TimeSpan m_dirtyTempTime;

		public TempFolderFactory(DirectoryInfo baseDir, TimeSpan dirtyTempTime)
		{
			m_baseDir = baseDir;
			m_dirtyTempTime = dirtyTempTime;
		}

		public TempFolder Create()
		{
			return new TempFolder(m_baseDir, m_dirtyTempTime);
		}
	}
}