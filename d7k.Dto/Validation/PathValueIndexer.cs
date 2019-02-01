using d7k.Utilities.Monads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace d7k.Dto
{
	public class PathValueIndexer<TSource>
	{
		Func<TSource, object[], object> m_get;
		Action<TSource, object[], object> m_set;
		Func<TSource, object[], bool> m_availability;
		Func<TSource, object[], object[]> m_indexes;

		int m_indexCount;

		List<PathItem> m_pathItems;
		public string GeneralPath { get; private set; }

		/// <summary>
		/// Please use First() for value indexing
		/// </summary>
		public static PathValueIndexer<TSource> Create<TProperty>(Expression<Func<TSource, TProperty>> getter)
		{
			var result = new PathValueIndexer<TSource>();

			result.m_pathItems = GetPathItems(getter);
			result.m_indexCount = result.m_pathItems.Where(x => x.Array != null).Count();

			result.m_get = GetFunctionBuilder<TSource>.GetFunction(result.m_pathItems);
			result.m_set = new SetFunctionBuilder<TSource>(result.m_pathItems).Build();
			result.m_availability = new ExistFunctionBuilder<TSource>(result.m_pathItems).Build();
			result.m_indexes = new IndexesFunctionBuilder<TSource>(result.m_pathItems).Build();

			result.GeneralPath = GetGeneralPath(result.m_pathItems);

			return result;
		}

		private static List<PathItem> GetPathItems<TProperty>(Expression<Func<TSource, TProperty>> getter)
		{
			var current = getter.Body;
			if (current.NodeType == ExpressionType.Convert)
				current = ((UnaryExpression)current).Operand;

			var pathItems = new List<PathItem>();

			while (current.NodeType != ExpressionType.Parameter)
			{
				if (current.NodeType == ExpressionType.MemberAccess)
				{
					var tMember = (MemberExpression)current;
					if (tMember.Member.MemberType != MemberTypes.Property && tMember.Member.MemberType != MemberTypes.Field)
						throw new InvalidOperationException($"Getter has incompatible path part '{tMember.Member.Name}'.");

					pathItems.Add(new PathItem() { Property = tMember.Member });
					current = tMember.Expression;
				}
				else if (current.NodeType == ExpressionType.Call)
				{
					var tMember = (MethodCallExpression)current;
					if (tMember.Method.Name != nameof(Enumerable.First))
						throw new InvalidOperationException($"Getter has incompatible path part '{tMember.Method.Name}'.");

					var listType = tMember.Arguments[0].Type.GetInterface("IList`1");
					var dictionaryType = tMember.Arguments[0].Type.GetInterface("IDictionary`2");

					if (listType != null && dictionaryType != null)
						throw new InvalidOperationException($"Getter has incompatible array part '{tMember.Method.Name}'. IDictionary and IList (and Array too) available only.");

					var curArray = tMember.Arguments[0];
					pathItems.Add(new PathItem() { Array = dictionaryType != null ? dictionaryType : listType });

					current = curArray;
				}
			}

			pathItems.Reverse();
			return pathItems;
		}

		private PathValueIndexer() { }

		private static string GetGeneralPath(List<PathItem> pathItems)
		{
			var pathAccum = new StringBuilder("");
			foreach (var t in pathItems)
				if (t.Property != null)
					pathAccum.Append(".").Append(t.Property.Name);
				else
					pathAccum.Append("[]");

			return pathAccum.ToString();
		}

		public static string GetPathName<TProperty>(Expression<Func<TSource, TProperty>> getter)
		{
			return GetGeneralPath(GetPathItems(getter));
		}

		public IEnumerable<PathValue<TSource>> GetPathes(TSource source)
		{
			var indexes = GetAllIndexes(source, new object[0]);
			foreach (var t in indexes)
				if (m_availability(source, t))
					yield return new PathValue<TSource>(source, t, m_get, m_set, m_availability, GetIndexedPath(t));
		}

		IEnumerable<object[]> GetAllIndexes(TSource source, object[] previousIndexes)
		{
			if (previousIndexes.Length == m_indexCount)
			{
				yield return previousIndexes;
				yield break;
			}

			var lastIndexes = m_indexes(source, previousIndexes);
			if (lastIndexes == null)
				yield break;

			foreach (var tIt in lastIndexes)
			{
				var newIndexes = previousIndexes.AddItem(tIt).ToArray();

				foreach (var t in GetAllIndexes(source, newIndexes))
					yield return t;
			}
		}

		string GetIndexedPath(object[] index)
		{
			int indexNum = 0;

			var pathAccum = new StringBuilder("");
			foreach (var t in m_pathItems)
				if (t.Property != null)
					pathAccum.Append(".").Append(t.Property.Name);
				else
				{
					var tIndex = index[indexNum];
					if (tIndex is string)
						tIndex = $"'{tIndex}'";

					pathAccum.Append("[").Append(tIndex).Append("]");
					indexNum++;
				}

			return pathAccum.ToString();
		}
	}

	public class PathValueIndexerFactory
	{
		public static PathValueIndexer<TSource> Create<TSource, TProperty>(TSource example, Expression<Func<TSource, TProperty>> getter)
		{
			return PathValueIndexer<TSource>.Create<TProperty>(getter);
		}
	}
}
