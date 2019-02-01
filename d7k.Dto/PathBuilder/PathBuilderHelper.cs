using System;
using System.Linq.Expressions;
using System.Reflection;

namespace d7k.Dto
{
	public static class PathBuilderHelper
	{
		public static Type GetMemberType(MemberInfo member)
		{
			if (member is PropertyInfo)
				return ((PropertyInfo)member).PropertyType;
			else if (member is FieldInfo)
				return ((FieldInfo)member).FieldType;
			else
				throw new NotImplementedException();
		}

		public static Expression GetIndexExpression(Expression indexes, int indexNum, Type arrayType)
		{
			Expression getIndex = Expression.ArrayIndex(indexes, new[] { Expression.Constant(indexNum) });

			if (arrayType.Name == "IDictionary`2")
				return Expression.Convert(getIndex, arrayType.GenericTypeArguments[0]);
			else
				return Expression.Convert(getIndex, typeof(int));
		}

		public static Expression ValueFromExpression(this MemberInfo member, Expression cur)
		{
			if (cur.Type != member.DeclaringType)
				cur = Expression.Convert(cur, member.DeclaringType);

			cur = Expression.PropertyOrField(cur, member.Name);

			return cur;
		}
	}
}