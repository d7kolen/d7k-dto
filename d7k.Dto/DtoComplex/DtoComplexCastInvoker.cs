namespace d7k.Dto
{
	class DtoComplexCastInvoker
	{
		DtoCopier m_copier;

		public DtoComplexCastInvoker(DtoCopier copier)
		{
			m_copier = copier;
		}

		public void CastValue<TDst, TSrc>(TSrc src, out TDst dst)
		{
			var tSrcWrp = new CastWrp<TSrc>() { Value = src };
			var tDstWrp = new CastWrp<TDst>();

			m_copier.Copy(tDstWrp, tDstWrp.GetType(), tSrcWrp, tSrcWrp.GetType(), false, null);

			dst = tDstWrp.Value;
		}

		public class CastWrp<T>
		{
			public T Value { get; set; }
		}
	}
}
