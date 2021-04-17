using System;

namespace d7k.Dto
{
	/// <summary>
	/// Available signatures:<para/>
	/// static void Convert(TDst dst, TSrc src)<para/>
	/// static void Convert(TDst dst, TSrc src, DtoComplex dto)<para/>
	/// static void Convert&lt;TDst0,...,TSrc0,...&gt;(TTempl&lt;TDst0,...&gt; dst, TTempl&lt;TSrc0,...&gt; src)<para/>
	/// static void Convert&lt;TDst0,...,TSrc0,...&gt;(TTempl&lt;TDst0,...&gt; dst, TTempl&lt;TSrc0,...&gt; src, DtoComplex dto)<para/>
	/// static void Convert&lt;TDst0,...&gt;(TTempl&lt;TDst0,...&gt; dst, TTempl&lt;TSrc0,...&gt; src)<para/>
	/// static void Convert&lt;TDst0,...&gt;(TTempl&lt;TDst0,...&gt; dst, TTempl&lt;TSrc0,...&gt; src, DtoComplex dto)<para/>
	/// static void Convert&lt;TSrc0,...&gt;(TTempl&lt;TDst0,...&gt; dst, TTempl&lt;TSrc0,...&gt; src)<para/>
	/// static void Convert&lt;TSrc0,...&gt;(TTempl&lt;TDst0,...&gt; dst, TTempl&lt;TSrc0,...&gt; src, DtoComplex dto)<para/>
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class DtoConvertAttribute : Attribute { }
}
