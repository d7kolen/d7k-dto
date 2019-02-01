using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace d7k.Dto
{
	class GetFunctionBuilder<TSource>
	{
		public static Func<TSource, object[], object> GetFunction(List<PathItem> pathItems)
		{
			var sourcePar = Expression.Parameter(typeof(TSource));
			var indexesPar = Expression.Parameter(typeof(object[]));

			var cur = GetFunctionExpression(sourcePar, indexesPar, pathItems);
			cur = Expression.Convert(cur, typeof(object));

			var delegateType = typeof(Func<,,>).MakeGenericType(typeof(TSource), typeof(object[]), typeof(object));

			return (Func<TSource, object[], object>)Expression.Lambda(delegateType, cur, sourcePar, indexesPar).Compile();
		}

		public static Expression GetFunctionExpression(ParameterExpression sourcePar, ParameterExpression indexesPar, List<PathItem> pathItems)
		{
			int indexNum = 0;
			Expression cur = sourcePar;

			foreach (var t in pathItems)
			{
				if (t.Property != null)
				{
					cur = t.Property.ValueFromExpression(cur);
				}
				else if (t.Array != null)
				{
					Expression getIndex = PathBuilderHelper.GetIndexExpression(indexesPar, indexNum, t.Array);

					cur = Expression.Convert(cur, t.Array);
					cur = Expression.MakeIndex(cur, t.Array.GetProperty("Item"), new[] { getIndex });

					indexNum++;
				}
			}

			return cur;
		}
	}
}
