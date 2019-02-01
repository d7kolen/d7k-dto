using d7k.Utilities.Monads;
using System;
using System.Collections.Generic;

namespace d7k.Dto
{
	public static class DtoCopierHelper
	{
		static DtoCopier m_copier = new DtoCopier();

		public static TDst ReadFrom<TSrc, TDst>(this TDst dst, TSrc src)
			where TDst : TSrc
		{
			m_copier.Copy(dst, src, typeof(TSrc), false, null);
			return dst;
		}

		public static TDst ReadFrom<TSrc, TDst>(this TDst dst, TSrc src, Type templateType)
		{
			m_copier.Copy(dst, src, templateType, false, null);
			return dst;
		}

		public static T CloneDto<T>(this T src) where T : class, new()
		{
			if (src == null)
				return default(T);

			return new T().ReadFrom(src);
		}

		public static IEnumerable<T> CloneList<T>(this IEnumerable<T> src) where T : new()
		{
			foreach (var t in src)
				yield return new T().ReadFrom(t);
		}

		public static TDst UpdateTo<TSrc, TDst>(this TSrc src, TDst dst, Type templateType, params string[] properties)
		{
			m_copier.Copy(dst, src, templateType, false, new HashSet<string>().Load(properties));
			return dst;
		}

		public static TDst UpdateWithExclude<TSrc, TDst>(this TSrc src, TDst dst, Type templateType, params string[] excludeProperties)
		{
			m_copier.Copy(dst, src, templateType, true, new HashSet<string>().Load(excludeProperties));
			return dst;
		}

		/// <summary>
		/// Prepare a delegate which has method signature: void CopyMethod(DstType dst, SrcType src, HashSet/<string/> properties)<para/>
		/// The delegate can take properties NULL value. In the case it will do full copy.
		/// </summary>
		public static Delegate CopyDelegate(Type dstType, Type srcType, Type templateType)
		{
			return m_copier.GetCopyDelegate(dstType, srcType, templateType, false);
		}
	}
}
