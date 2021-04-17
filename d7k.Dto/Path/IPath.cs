using System.Collections.Generic;

namespace d7k.Dto
{
	public interface IPath<TSource>
	{
		void PrepareForIndexing();
		object Get(TSource source, object[] index);
		void Set(TSource source, object[] index, object value);

		string PathName();
		string IndexedName(object[] index);

		IEnumerable<object[]> GetAllIndexes(TSource source);
	}
}