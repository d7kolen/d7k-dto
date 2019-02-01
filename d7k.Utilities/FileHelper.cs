using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace d7k.Utilities
{
	public static class FileHelper
	{
		private static void CopyDirectoryStructure(DirectoryInfo src, DirectoryInfo dst)
		{
			Debug.Assert(src != null && dst != null);

			if (!dst.Exists)
				dst.Create();

			foreach (var dir in src.GetDirectories())
				CopyDirectoryStructure(dir, new DirectoryInfo(dst.GetFullName(dir.Name)));
		}

		public static string GetFullName(this DirectoryInfo dir, string name)
		{
			return dir.FullName.AppendDirectorySeparator() + name;
		}

		public static string NameOnly(this FileInfo file)
		{
			return Path.GetFileNameWithoutExtension(file.FullName);
		}

		public static FileInfo SubFile(this DirectoryInfo parent, string name)
		{
			return new FileInfo(parent.GetFullName(name));
		}

		public static FileInfo SubFile(this DirectoryInfo parent, string name, string extension)
		{
			return new FileInfo(parent.GetFullName(string.Format("{0}.{1}", name, extension)));
		}

		public static bool IsFullPath(string path)
		{
			return Path.GetPathRoot(path) != string.Empty;
		}

		public static string SubPath(this DirectoryInfo baseDir, FileInfo file)
		{
			var fileName = file.FullName;
			var dirName = baseDir.FullName;

			if (fileName.StartsWith(dirName))
				if (file.FullName[dirName.Length] == Path.DirectorySeparatorChar)
					return fileName.Substring(dirName.Length + 1);
				else
					return fileName.Substring(dirName.Length);
			else
				return file.FullName;
		}

		public static FileInfo CompilePath(this DirectoryInfo baseDir, string partPath)
		{
			if (string.IsNullOrEmpty(partPath))
				return null;

			if (FileHelper.IsFullPath(partPath))
				return new FileInfo(partPath);
			else
				return baseDir.SubFile(partPath);
		}

		public static FileInfo ChangeExtension(this FileInfo file, string newExtension)
		{
			return new FileInfo(Path.ChangeExtension(file.FullName, newExtension));
		}

		public static DirectoryInfo SubDir(this DirectoryInfo parent, string name)
		{
			return new DirectoryInfo(parent.GetFullName(name));
		}

		public static FileInfo TestDir(this FileInfo file)
		{
			if (!file.Directory.Exists)
				file.Directory.Create();

			return file;
		}

		public static void CopyDirTo(this DirectoryInfo src, DirectoryInfo dst, bool overwrite)
		{
			CopyDirectoryStructure(src, dst);

			foreach (var file in src.GetFiles("*.*", SearchOption.AllDirectories))
			{
				var result = new FileInfo(dst.GetFullName(file.FullName.Substring(src.FullName.Length + 1)));

				if (!result.Exists || result.LastWriteTime < file.LastWriteTime || overwrite)
					file.CopyFileTo(result, true);
			}
		}

		public static void CopyFileTo(this FileInfo src, FileInfo dst, bool ignoreReadonlyState)
		{
			try
			{
				if (dst.Exists)
				{
					if (ignoreReadonlyState)
						dst.ClearReadOnly();

					dst.Delete();
				}

				src.CopyTo(dst.FullName, true);
			}
			catch { /* ??? */ }
		}

		public static void DeleteDir(this DirectoryInfo dir, bool ignoreReadonlyState)
		{
			if (dir.Exists)
			{
				if (ignoreReadonlyState)
				{
					dir.GetFiles("*", SearchOption.AllDirectories).ClearReadOnly();
					dir.GetDirectories("*", SearchOption.AllDirectories).ClearReadOnly();
				}

				try
				{
					if (ignoreReadonlyState)
						dir.ClearReadOnly();

					dir.Delete(true);
				}
				catch { /* Это текущая папка для какой-либо программы? */ }
			}
		}

		public static void DeleteFile(this FileInfo file, bool ignoreReadonlyState)
		{
			if (file.Exists)
			{
				if (ignoreReadonlyState)
					file.ClearReadOnly();

				file.Delete();
			}
		}

		static void ClearReadOnly(this FileSystemInfo fsi)
		{
			new[] { fsi }.ClearReadOnly();
		}

		static void ClearReadOnly(this IEnumerable<FileSystemInfo> fsi)
		{
			foreach (var t in fsi)
				t.Attributes = FileAttributes.Normal;
		}

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern uint GetShortPathName
		(
			[MarshalAs(UnmanagedType.LPTStr)]
			string lpszLongPath,

			[MarshalAs(UnmanagedType.LPTStr)]
			StringBuilder lpszShortPath,

			uint cchBuffer
		);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern uint GetLongPathName
		(
			[MarshalAs(UnmanagedType.LPTStr)]
			string lpszShortPath,

			[MarshalAs(UnmanagedType.LPTStr)]
			StringBuilder lpszLongPath,

			uint cchBuffer
		);

		public static FileInfo ExecutingAssemblyFile()
		{
			return new FileInfo(Assembly.GetCallingAssembly().Location);
		}

		public static DirectoryInfo ExecutingAssemblyDir()
		{
			return new FileInfo(Assembly.GetCallingAssembly().Location).Directory;
		}

		public static string AppendDirectorySeparator(this string path)
		{
			var s = Path.DirectorySeparatorChar.ToString();
			return path.EndsWith(s) ? path : path + s;
		}
	}
}
