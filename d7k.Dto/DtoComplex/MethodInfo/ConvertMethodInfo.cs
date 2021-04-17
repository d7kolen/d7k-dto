using System;
using System.Reflection;

namespace d7k.Dto.Complex
{
	class ConvertMethodInfo
	{
		public MethodInfo Method { get; set; }
		public Type DstType { get; set; }
		public Type SrcType { get; set; }
		public bool HasContext { get; set; }

		void Invoke(object dst, object src, object context)
		{
			object[] parameters;
			if (HasContext)
				parameters = new[] { dst, src, context };
			else
				parameters = new[] { dst, src };

			Method.Invoke(null, parameters);
		}

		public void Invoke(object dst, object src, object context, UpdationListWriter writer)
		{
			if (writer?.UpdationList == null)
			{
				InvokeWithoutUpdater(dst, src, context);
				return;
			}

			var adaptedDst = GetDtoDst();
			var adaptedSrc = GetSrcAdapter(src);

			Invoke(adaptedDst, adaptedSrc, context);

			writer.Copy(dst, adaptedDst, DstType);
		}

		private void InvokeWithoutUpdater(object dst, object src, object context)
		{
			var adaptedDst = GetDstAdapter(dst);
			var adaptedSrc = GetSrcAdapter(src);

			Invoke(adaptedDst, adaptedSrc, context);
		}

		public object GetSrcAdapter(object src)
		{
			if (SrcType == src.GetType())
				return src;
			return DtoFactory.DtoAdapter(src, SrcType);
		}

		public object GetDtoDst()
		{
			if (DstType.IsInterface)
				return DtoFactory.Dto(DstType);
			return Activator.CreateInstance(DstType);
		}

		public object GetDstAdapter<TDst>(TDst dst)
		{
			if (DstType.IsInterface)
				return DtoFactory.DtoAdapter(dst, DstType);
			else
				return dst;
		}
	}
}