using System;
using System.Collections.Generic;
using System.Linq;

namespace d7k.Utilities.Monads
{
	public static class MonadHelper
	{
		public static IEnumerable<T> AddItem<T>(this IEnumerable<T> list, T item)
		{
			if (list != null)
				foreach (var t in list)
					yield return t;

			yield return item;
		}

		public static TValue Get<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key) where TValue : class
		{
			if (dict == null)
				return null;

			TValue res;
			return dict.TryGetValue(key, out res) ? res : null;
		}

		public static IEnumerable<T> DefaultIfNull<T>(this IEnumerable<T> source)
		{
			if (source == null)
				return Enumerable.Empty<T>();
			return source;
		}

		public static TValue Default<TValue>(this TValue val, Func<TValue> getDefaultValue)
		{
			if (object.Equals(val, default(TValue)))
				return getDefaultValue();
			else
				return val;
		}

		public static TValue Default<TValue>(this TValue val, TValue defaultValue)
		{
			if (object.Equals(val, default(TValue)))
				return defaultValue;
			else
				return val;
		}

		public static IEnumerable<IEnumerable<T>> ToBatches<T>(this IEnumerable<T> list, int batchLen)
		{
			var finish = new List<bool>();

			using (var en = list.GetEnumerator())
			{
				finish.Add(!en.MoveNext());

				while (!finish[0])
					yield return TakeBatch(en, batchLen, finish);
			}
		}

		static IEnumerable<T> TakeBatch<T>(this IEnumerator<T> list, int batchLen, List<bool> finish)
		{
			int count = 0;
			while (batchLen > count)
			{
				yield return list.Current;
				count++;

				if (!list.MoveNext())
				{
					finish[0] = true;
					yield break;
				}
			}
		}

		public static Dictionary<TKey, TValue> Load<TKey, TValue>(this Dictionary<TKey, TValue> accum, IEnumerable<TValue> src, Func<TValue, TKey> getKey)
		{
			foreach (var t in src ?? Enumerable.Empty<TValue>())
				accum[getKey(t)] = t;

			return accum;
		}

		public static HashSet<TValue> Load<TValue>(this HashSet<TValue> accum, IEnumerable<TValue> src)
		{
			foreach (var t in src ?? Enumerable.Empty<TValue>())
				accum.Add(t);

			return accum;
		}

		public static Dictionary<string, string> LoadUrlParameters(this Dictionary<string, string> parameters, Uri url)
		{
			foreach (var t in url.Query.TrimStart('?').Split('&'))
			{
				var items = t.Trim().Split('=');
				parameters[items[0]] = items[1];
			}

			return parameters;
		}
	}
}
