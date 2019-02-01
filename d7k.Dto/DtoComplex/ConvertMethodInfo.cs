using System;
using System.Reflection;

namespace d7k.Dto
{
	class ConvertMethodInfo
	{
		public MethodInfo Method { get; set; }
		public Type DstType { get; set; }
		public Type SrcType { get; set; }
		public bool HasContext { get; set; }

		public void Invoke(object dst, object src, object context)
		{
			object[] parameters;
			if (HasContext)
				parameters = new[] { dst, src, context };
			else
				parameters = new[] { dst, src };

			Method.Invoke(null, parameters);
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

		public object GetAdaptedDst<TDst>(TDst dst)
		{
			if (DstType.IsInterface)
				return DtoFactory.DtoAdapter(dst, DstType);
			else
				return dst;
		}
	}
}