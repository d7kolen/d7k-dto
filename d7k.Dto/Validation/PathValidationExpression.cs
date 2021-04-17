using System;
using System.Collections.Generic;
using System.Linq;

namespace d7k.Dto
{
	public static class PathValidationExpression
	{
		/// <summary>
		/// Method for path validation IEnumerable. Don't call it!
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		public static TSource ScanAll<TSource>(this IEnumerable<TSource> source)
		{
			throw new NotImplementedException("This is method for expression. Don't call it!");
		}
	}
}
