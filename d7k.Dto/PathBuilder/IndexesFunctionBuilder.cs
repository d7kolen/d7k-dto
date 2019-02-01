using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace d7k.Dto
{
	class IndexesFunctionBuilder<TSource>
	{
		static MethodInfo s_toArrayKeys = typeof(Enumerable).GetMethods()
			.Where(x => x.Name == nameof(Enumerable.ToArray) && x.GetParameters().Length == 1)
			.Select(x => x.MakeGenericMethod(typeof(object)))
			.First();

		static MethodInfo s_castKeys = typeof(Enumerable).GetMethods()
			.Where(x => x.Name == nameof(Enumerable.Cast) && x.GetParameters().Length == 1)
			.Select(x => x.MakeGenericMethod(typeof(object)))
			.First();

		static MethodInfo s_rangeEnum = typeof(Enumerable).GetMethods()
			.Where(x => x.Name == nameof(Enumerable.Range) && x.GetParameters().Length == 2)
			.First();

		List<PathItem> m_pathItems;

		LabelTarget m_returnLabel;
		List<Expression> m_block;
		List<ParameterExpression> m_variables;

		public IndexesFunctionBuilder(List<PathItem> pathItems)
		{
			m_pathItems = pathItems;

			m_returnLabel = Expression.Label(typeof(object[]));

			m_block = new List<Expression>();
			m_variables = new List<ParameterExpression>();
		}

		public Func<TSource, object[], object[]> Build()
		{
			var sourcePar = Expression.Parameter(typeof(TSource));
			var indexPar = Expression.Parameter(typeof(object[]));

			Expression cur = sourcePar;
			int indexNum = 0;

			foreach (var t in m_pathItems)
			{
				AddCheck(cur);

				if (t.Property != null)
					cur = ForProperty(cur, t.Property);
				else if (t.Array != null)
				{
					cur = ForArray(cur, t.Array, indexPar, indexNum);
					indexNum++;
				}
			}

			m_block.Add(Expression.Label(m_returnLabel, Expression.Constant(null, typeof(object[]))));
			var block = Expression.Block(m_variables, m_block);

			return (Func<TSource, object[], object[]>)Expression.Lambda(block, sourcePar, indexPar).Compile();
		}

		private Expression ForArray(Expression cur, Type tArr, ParameterExpression indexPar, int indexNum)
		{
			var indexLeng = Expression.PropertyOrField(indexPar, nameof(Array.Length));

			Expression keys;
			if (tArr.Name == "IDictionary`2")
				keys = Expression.PropertyOrField(cur, "Keys");
			else
			{
				var countKeys = Expression.PropertyOrField(Expression.Convert(cur, typeof(System.Collections.ICollection)), nameof(System.Collections.ICollection.Count));
				keys = Expression.Call(s_rangeEnum, Expression.Constant(0), countKeys);
			}

			keys = Expression.Call(s_castKeys, keys);
			keys = Expression.Call(s_toArrayKeys, keys);

			m_block.Add(Expression.IfThen(
				Expression.LessThanOrEqual(indexLeng, Expression.Constant(indexNum)),
				Expression.Return(m_returnLabel, keys)));

			var getIndex = PathBuilderHelper.GetIndexExpression(indexPar, indexNum, tArr);

			cur = Expression.MakeIndex(cur, tArr.GetProperty("Item"), new[] { getIndex });

			var arrVariable = Expression.Variable(cur.Type);
			m_block.Add(Expression.Assign(arrVariable, cur));
			m_variables.Add(arrVariable);
			m_block.Add(arrVariable);

			cur = arrVariable;

			return cur;
		}

		private Expression ForProperty(Expression cur, MemberInfo property)
		{
			cur = property.ValueFromExpression(cur);
			var variable = Expression.Variable(PathBuilderHelper.GetMemberType(property));
			m_variables.Add(variable);
			m_block.Add(variable);

			m_block.Add(Expression.Assign(variable, cur));
			cur = variable;
			return cur;
		}

		void AddCheck(Expression cur)
		{
			var tCheck = Expression.IfThen(
				Expression.Equal(cur, Expression.Constant(null)),
				Expression.Return(m_returnLabel, Expression.Constant(null, typeof(object[]))));

			m_block.Add(tCheck);
		}
	}
}
