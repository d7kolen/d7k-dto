using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace d7k.Utilities
{
	public interface IWeakDictionaryDbg
	{
		int DictCount { get; }
	}

	public class WeakDictionary<TKey, TValue> : IWeakDictionaryDbg
		where TValue : class
	{
		Dictionary<IWeakValue<TKey>, IWeakValue<TValue>> m_dict;
		Func<TKey, IWeakValue<TKey>> m_keyFactory;
		Func<TValue, IWeakValue<TValue>> m_valueFactory;

		public WeakDictionary()
			: this(x => new WeakValue<TKey>(x), x => new WeakValue<TValue>(x))
		{
		}

		WeakDictionary(Func<TKey, IWeakValue<TKey>> keyFactory, Func<TValue, IWeakValue<TValue>> valueFactory)
		{
			m_keyFactory = keyFactory;
			m_valueFactory = valueFactory;
			m_dict = new Dictionary<IWeakValue<TKey>, IWeakValue<TValue>>();
		}

		public IEnumerable<TKey> Keys
		{
			get
			{
				return from x in m_dict
					   let res = x.Key.Value
					   where res != null && x.Value.Value != null
					   select res;
			}
		}

		public IEnumerable<TValue> Values
		{
			get
			{
				return from x in m_dict
					   let res = x.Value.Value
					   where x.Key.Value != null && x.Value.Value != null
					   select res;
			}
		}

		public TValue this[TKey key]
		{
			get
			{
				TValue res;
				if (!TryGetValue(key, out res))
					throw new KeyNotFoundException();
				return res;
			}
			set
			{
				Debug.Assert(value != null && key != null);
				m_dict[m_keyFactory(key)] = m_valueFactory(value);
			}
		}

		public bool TryGetValue(TKey key, out TValue res)
		{
			res = null;

			if (key == null)
				return false;

			IWeakValue<TValue> t;

			if (m_dict.TryGetValue(m_keyFactory(key), out t))
				return (res = t.Value) != null;

			return false;
		}

		public void Remove(TKey key)
		{
			m_dict.Remove(new WeakValue<TKey>(key));
		}

		public void Trim()
		{
			var fromRemove = (from x in m_dict
							  where x.Key.Value == null || x.Value.Value == null
							  select x.Key).
							  ToList();

			// для объектов совподающих по ссылкам Equals сработает
			// в независимости от существования ключа
			foreach (var t in fromRemove)
				m_dict.Remove(t);
		}

		public static WeakDictionary<TKey, TValue> CreateWeakValue()
		{
			return new WeakDictionary<TKey, TValue>(x => new NotWeakValue<TKey>(x), x => new WeakValue<TValue>(x));
		}

		public static WeakDictionary<TKey, TValue> CreateWeakKey()
		{
			return new WeakDictionary<TKey, TValue>(x => new WeakValue<TKey>(x), x => new NotWeakValue<TValue>(x)); ;
		}

		#region IWeakDictionaryDbg Members

		int IWeakDictionaryDbg.DictCount
		{
			get { return m_dict.Count; }
		}

		#endregion
	}

	interface IWeakValue<TValue>
	{
		TValue Value { get; }
	}

	class NotWeakValue<TValue> : IWeakValue<TValue>
	{
		TValue m_value;

		public TValue Value { get { return m_value; } }

		public NotWeakValue(TValue value)
		{
			m_value = value;
		}

		public override int GetHashCode()
		{
			return m_value.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			var val = obj as NotWeakValue<TValue>;
			return val != null && m_value.Equals(val.m_value);
		}
	}

	class WeakValue<TValue> : IWeakValue<TValue>
	{
		int m_hash;
		WeakReference m_value;

		public TValue Value { get { return (TValue)m_value.Target; } }

		public WeakValue(TValue value)
		{
			m_hash = value.GetHashCode();
			m_value = new WeakReference(value);
		}

		public override int GetHashCode()
		{
			return m_hash;
		}

		public override bool Equals(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;

			var trg = m_value.Target;
			var argValue = obj as WeakValue<TValue>;

			if (argValue == null || trg == null)
				return false;

			return trg.Equals(argValue.m_value.Target);
		}
	}
}
