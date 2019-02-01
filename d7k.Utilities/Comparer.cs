using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace d7k.Utilities
{
	public class FuncComparer<T> : Comparer<T>
	{
		private Func<T, T, int> _compare;

		public FuncComparer(Func<T, T, int> func)
		{
			_compare = func;
		}

		public static FuncComparer<T> Create<T1>(Func<T, T1> ratingSelector)
		{
			return new FuncComparer<T>(
				(x, y) => Comparer<T1>.Default.Compare(ratingSelector(x), ratingSelector(y)));
		}

		public int Compare(object x, object y)
		{
			return _compare((T)x, (T)y);
		}

		public override int Compare(T x, T y)
		{
			return _compare(x, y);
		}
	}

	public class FuncEqualityComparer<T> : IEqualityComparer<T>
	{
		Func<T, T, bool> m_equels;
		Func<T, int> m_hash;

		public FuncEqualityComparer(Func<T, object> getKey) :
			this((x0, x1) => object.Equals(getKey(x0), getKey(x1)), x => getKey(x).GetHashCode())
		{
		}

		public FuncEqualityComparer(Func<T, T, bool> equels, Func<T, int> hash)
		{
			m_equels = equels;
			m_hash = hash;
		}

		public bool Equals(T x, T y)
		{
			return m_equels(x, y);
		}

		public int GetHashCode(T obj)
		{
			return m_hash(obj);
		}
	}
}
