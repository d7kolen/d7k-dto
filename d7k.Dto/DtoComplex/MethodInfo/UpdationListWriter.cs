using System;
using System.Collections.Generic;

namespace d7k.Dto
{
	class UpdationListWriter
	{
		DtoCopier m_copier;
		public HashSet<string> UpdationList { get; }

		public UpdationListWriter(DtoCopier copier, HashSet<string> updationList)
		{
			m_copier = copier;
			UpdationList = updationList;
		}

		public void Copy(object dst, object src, Type templateType)
		{
			m_copier.Copy(dst, src, templateType, false, UpdationList);
		}

		public void Copy(object dst, Type dstTemplate, object src, Type srcTemplate)
		{
			m_copier.Copy(dst, dstTemplate, src, srcTemplate, false, UpdationList);
		}
	}

	static class UpdationListWriterHelper
	{
		public static UpdationListWriter Updater(this DtoCopier copier, HashSet<string> updationList)
		{
			return new UpdationListWriter(copier, updationList);
		}
	}
}
