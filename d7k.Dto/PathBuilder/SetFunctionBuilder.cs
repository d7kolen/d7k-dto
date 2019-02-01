using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace d7k.Dto
{
	class SetFunctionBuilder<TSource>
	{
		static MethodInfo s_lastIndex = typeof(Enumerable).GetMethods()
			.Where(x => x.Name == nameof(Enumerable.Last) && x.GetParameters().Length == 1)
			.Select(x => x.MakeGenericMethod(typeof(object)))
			.First();

		List<PathItem> m_pathItems;

		public SetFunctionBuilder(List<PathItem> pathItems)
		{
			m_pathItems = pathItems;
		}

		public Action<TSource, object[], object> Build()
		{
			if (!m_pathItems.Any())
				return (TSource source, object[] indexes, object value) => { };

			var lastPath = m_pathItems.Last();

			if (lastPath.Array != null)
				return ForArray(lastPath.Array);

			var result = ForProperty(lastPath.Property);

			if (result == null)
				throw new NotImplementedException();

			return result;
		}

		private Action<TSource, object[], object> ForProperty(MemberInfo lastMember)
		{
			var parSource = Expression.Parameter(typeof(TSource));
			var getBase = GetFunctionBuilder<TSource>.GetFunction(m_pathItems.Take(m_pathItems.Count - 1).ToList());

			if (lastMember is FieldInfo)
			{
				var fieldLastMember = (FieldInfo)lastMember;
				return (source, indexes, value) => fieldLastMember.SetValue(getBase(source, indexes), value);
			}

			var propLastMember = (PropertyInfo)lastMember;
			if (propLastMember != null && propLastMember.CanWrite)
				return (source, indexes, value) => propLastMember.SetValue(getBase(source, indexes), value);

			if (propLastMember != null && !propLastMember.CanWrite)
			{
				var backFieldName = $"<{propLastMember.Name}>i__Field";

				var backField = propLastMember
					.DeclaringType
					.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
					.FirstOrDefault(f => f.Name == backFieldName);

				return (source, indexes, value) => backField.SetValue(getBase(source, indexes), value);
			}

			return null;
		}

		private Action<TSource, object[], object> ForArray(Type lastArray)
		{
			var parSource = Expression.Parameter(typeof(TSource));
			var parIndexes = Expression.Parameter(typeof(object[]));
			var parValue = Expression.Parameter(typeof(object));
			Expression newValue;

			Expression lastIndexValue = Expression.Call(s_lastIndex, Expression.Convert(parIndexes, typeof(IEnumerable<object>)));

			if (lastArray.Name == "IDictionary`2")
			{
				lastIndexValue = Expression.Convert(lastIndexValue, lastArray.GenericTypeArguments[0]);
				newValue = Expression.Convert(parValue, lastArray.GenericTypeArguments[1]);
			}
			else
			{
				lastIndexValue = Expression.Convert(lastIndexValue, typeof(int));
				newValue = Expression.Convert(parValue, lastArray.GenericTypeArguments[0]);
			}

			var baseExpress = GetFunctionBuilder<TSource>.GetFunctionExpression(parSource, parIndexes, m_pathItems.Take(m_pathItems.Count - 1).ToList());

			var body = Expression.Assign(
				Expression.MakeIndex(baseExpress, lastArray.GetProperty("Item"), new[] { lastIndexValue }),
				newValue);

			return (Action<TSource, object[], object>)Expression.Lambda(typeof(Action<TSource, object[], object>),
				body, parSource, parIndexes, parValue).Compile();
		}
	}
}
