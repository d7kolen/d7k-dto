using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace d7k.Dto
{
	public class ObjectFieldsPathFactory
	{
		public IPath<TSource> GetPathItems<TSource, TProperty>(Expression<Func<TSource, TProperty>> getter)
		{
			var current = getter.Body;
			if (current.NodeType == ExpressionType.Convert)
				current = ((UnaryExpression)current).Operand;

			var pathItems = new List<PathItem>();

			while (current.NodeType != ExpressionType.Parameter)
			{
				if (current.NodeType == ExpressionType.MemberAccess)
					current = AddMemberField(current, pathItems);
				else if (current.NodeType == ExpressionType.Call)
					current = AddCallField(current, pathItems);
			}

			pathItems.Reverse();

			return new ObjectFieldsPath<TSource>(pathItems);
		}

		Expression AddCallField(Expression current, List<PathItem> pathItems)
		{
			var compatibleMethods = new[] { nameof(Enumerable.First), nameof(PathValidationExpression.ScanAll) };

			var tMember = (MethodCallExpression)current;
			if (!compatibleMethods.Contains(tMember.Method.Name))
				throw new InvalidOperationException($"Getter has incompatible path part '{tMember.Method.Name}'.");

			var listType = tMember.Arguments[0].Type.GetInterface("IList`1");
			var dictionaryType = tMember.Arguments[0].Type.GetInterface("IDictionary`2");

			if (listType != null && dictionaryType != null)
				throw new InvalidOperationException($"Getter has incompatible array part '{tMember.Method.Name}'. IDictionary and IList (and Array too) available only.");

			var curArray = tMember.Arguments[0];
			pathItems.Add(new PathItem() { Array = dictionaryType != null ? dictionaryType : listType });

			current = curArray;
			return current;
		}

		Expression AddMemberField(Expression current, List<PathItem> pathItems)
		{
			var tMember = (MemberExpression)current;
			if (tMember.Member.MemberType != MemberTypes.Property && tMember.Member.MemberType != MemberTypes.Field)
				throw new InvalidOperationException($"Getter has incompatible path part '{tMember.Member.Name}'.");

			pathItems.Add(new PathItem() { Property = tMember.Member });
			current = tMember.Expression;
			return current;
		}
	}
}
