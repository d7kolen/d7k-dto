using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace d7k.Utilities
{
	public static class PtrHelper
	{
		public static IntPtr AllocPtr(this object obj)
		{
			var ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf(obj));
			Marshal.StructureToPtr(obj, ptr, false);
			return ptr;
		}

		public static IEnumerable<IntPtr> AllocPtr<T>(this IEnumerable<T> value)
		{
			var result = new List<IntPtr>();
			foreach (var t in value)
				result.Add(t.AllocPtr());

			return result;
		}

		public static IntPtr AllocPtr(this IEnumerable<IntPtr> value)
		{
			var sizePtr = Marshal.SizeOf(typeof(IntPtr));
			var result = Marshal.AllocCoTaskMem((int)value.Count() * sizePtr);
			var cur = result;

			foreach (var t in value)
			{
				Marshal.WriteIntPtr(cur, t);
				cur = cur.MoveTo(sizePtr);
			}

			return result;
		}

		public static IntPtr AllocPtr(this byte[] value)
		{
			var res = Marshal.AllocCoTaskMem(value.Length);
			Marshal.Copy(value, 0, res, value.Length);
			return res;
		}

		public static void InitPtr<T>(this IEnumerable<T> value, IntPtr ptr)
		{
			var cur = ptr;
			var size = Marshal.SizeOf(typeof(T));

			foreach (var t in value)
			{
				Marshal.StructureToPtr(t, cur, false);
				cur = cur.MoveTo(size);
			}
		}

		public static T ExtractStruct<T>(this IntPtr ptr)
		{
			return (T)Marshal.PtrToStructure(ptr, typeof(T));
		}

		public static IEnumerable<T> ExtractArr<T>(this IntPtr ptr, int len)
		{
			var sizeT = Marshal.SizeOf(typeof(T));

			var list = new List<T>();
			for (int i = 0; i < len; i++)
				list.Add(ptr.MoveTo(sizeT * i).ExtractStruct<T>());
			return list;
		}

		public static IEnumerable<T> ExtractArr<T>(this IEnumerable<IntPtr> ptrs)
		{
			var sizeT = Marshal.SizeOf(typeof(IntPtr));

			var list = new List<T>();
			foreach (var t in ptrs)
				list.Add(t.ExtractStruct<T>());

			return list;
		}

		public static byte[] ExtractBytes(this IntPtr ptr, int len)
		{
			var res = new byte[len];
			Marshal.Copy(ptr, res, 0, (int)len);
			return res;
		}

		public static IntPtr MoveTo(this IntPtr ptr, int offset)
		{
			return new IntPtr(ptr.ToInt64() + offset);
		}
	}
}
