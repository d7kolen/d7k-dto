using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace d7k.Dto
{
	public class ExistFunctionBuilder<TSource>
	{
		List<PathItem> m_pathItems;

		LabelTarget m_returnLabel;
		List<Expression> m_block;
		List<ParameterExpression> m_variables;

		public ExistFunctionBuilder(List<PathItem> pathItems)
		{
			m_pathItems = pathItems;

			m_returnLabel = Expression.Label(typeof(bool));
			m_block = new List<Expression>();
			m_variables = new List<ParameterExpression>();
		}

		public Func<TSource, object[], bool> Build()
		{
			int indexNum = 0;

			var sourcePar = Expression.Parameter(typeof(TSource));
			var indexPar = Expression.Parameter(typeof(object[]));

			Expression cur = sourcePar;

			foreach (var t in m_pathItems)
			{
				AddCheck(cur);

				if (t.Property != null)
				{
					cur = t.Property.ValueFromExpression(cur);
				}
				else if (t.Array != null)
				{
					cur = ForArray(cur, t.Array, indexPar, indexNum);
					indexNum++;
				}
			}

			m_block.Add(Expression.Label(m_returnLabel, Expression.Constant(true)));

			return (Func<TSource, object[], bool>)Expression.Lambda(Expression.Block(m_variables, m_block), sourcePar, indexPar).Compile();
		}

		Expression ForProperty(Expression cur, MemberInfo property)
		{
			cur = property.ValueFromExpression(cur);

			var variable = Expression.Variable(PathBuilderHelper.GetMemberType(property));
			m_variables.Add(variable);
			m_block.Add(variable);
			m_block.Add(Expression.Assign(variable, cur));
			cur = variable;

			return cur;
		}

		Expression ForArray(Expression cur, Type arrayType, ParameterExpression arrayIndexesParam, int arrayIndexNum)
		{
			var getIndex = PathBuilderHelper.GetIndexExpression(arrayIndexesParam, arrayIndexNum, arrayType);

			cur = Expression.Convert(cur, arrayType);
			cur = Expression.MakeIndex(cur, arrayType.GetProperty("Item"), new[] { getIndex });

			var variable = Expression.Variable(cur.Type);
			m_variables.Add(variable);
			m_block.Add(variable);
			m_block.Add(Expression.Assign(variable, cur));

			cur = variable;

			return cur;
		}

		private void AddCheck(Expression cur)
		{
			var tCheck = Expression.IfThen(
				Expression.Equal(cur, Expression.Constant(null)),
				Expression.Return(m_returnLabel, Expression.Constant(false)));

			m_block.Add(tCheck);
		}
	}
}
