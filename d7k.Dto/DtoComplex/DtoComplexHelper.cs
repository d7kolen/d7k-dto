using System;

namespace d7k.Dto
{
	public static class DtoComplexHelper
	{
		public static TRes AsStrongly<TRes>(this DtoComplex complex, object obj)
		{
			var result = complex.As<TRes>(obj);
			if (result == null)
				throw new NotImplementedException($"{obj.GetType().FullName} type doesn't implement {typeof(TRes).FullName}.");

			return result;
		}

		public static T ByNestedClasses<T>(this T source, params Type[] classContainers) where T : DtoComplex
		{
			source.InitByNestedClasses(classContainers);
			return source;
		}

		public static object GetDtoAdapterSource(this DtoComplex complex, object obj)
		{
			if (obj is IDtoAdapter)
				return ((IDtoAdapter)obj).GetSource();
			return obj;
		}

		public static TDst CopyFrom<TDst, TSrc>(this TDst dst, TSrc src, DtoComplex complex)
		{
			return complex.Copy(dst, src);
		}
	}
}
