using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;

namespace d7k.Utilities
{
	public static class NameOf
	{
		public static string nameof<T, TProp>(this T obj, Expression<Func<T, TProp>> expression)
		{
			return ExtractMember(expression.Body).Name;
		}

		public static string nameof<T>(Expression<Func<T, object>> expression)
		{
			return ExtractMember(expression.Body).Name;
		}

		public static string nameof<T>(Expression<Func<T>> expression)
		{
			return ExtractMember(expression.Body).Name;
		}
		
		public static string pathof<T, TProp>(this T obj, Expression<Func<T, TProp>> expression)
		{
			return ExtractPath(expression.Body);
		}

		public static string pathof<T>(Expression<Func<T, object>> expression)
		{
			return ExtractPath(expression.Body);
		}

		public static string pathof<T>(LambdaExpression expression)
		{
			return ExtractPath(expression.Body);
		}

		public static PropertyInfo propertyof<T, TProp>(this T target, Expression<Func<T, TProp>> expression)
		{
			return (PropertyInfo)ExtractMember(expression.Body);
		}

		public static PropertyInfo propertyof<T>(Expression<Func<T, object>> expression)
		{
			return (PropertyInfo)ExtractMember(expression.Body);
		}

		public static PropertyInfo propertyof(Expression<Func<object>> expression)
		{
			return (PropertyInfo)ExtractMember(expression.Body);
		}

		static string ExtractPath(Expression expression)
		{
			expression = CleanConvert(expression);

			var accum = new Stack<MemberInfo>();

			while (expression is MemberExpression)
			{
				var t = (MemberExpression)expression;
				accum.Push(t.Member);
				expression = t.Expression;
			}

			var bldr = new StringBuilder();
			while (accum.Count > 0)
			{
				bldr.Append(accum.Pop().Name);
				bldr.Append(".");
			}

			if (bldr.Length > 0)
				bldr.Length--;

			return bldr.ToString();
		}

		static MemberInfo ExtractMember(Expression expression)
		{
			expression = CleanConvert(expression);

			var memberExp = expression as MemberExpression;
			if (memberExp != null)
				return memberExp.Member;

			throw new NotImplementedException();
		}

		static Expression CleanConvert(Expression expression)
		{
			if (expression.NodeType == ExpressionType.Convert)
				expression = ((UnaryExpression)expression).Operand;
			return expression;
		}
	}
}
