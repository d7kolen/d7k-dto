namespace d7k.Utilities
{
	public class TextSync
	{
		const int c_refreshStep = 1000;

		WeakDictionary<string, object> m_lock = WeakDictionary<string, object>.CreateWeakValue();

		public object GetSyncObject(string name)
		{
			if (name == null || string.IsNullOrEmpty(name.Trim()))
				return new object();

			object sync;
			if (m_lock.TryGetValue(name, out sync))
				return sync;

			lock (m_lock)
			{
				if (m_lock.TryGetValue(name, out sync))
					return sync;

				var dbg = (IWeakDictionaryDbg)m_lock;
				var prevCount = dbg.DictCount;

				var result = m_lock[name] = new object();

				if ((prevCount / c_refreshStep) != (dbg.DictCount / c_refreshStep))
					m_lock.Trim();

				return result;
			}
		}
	}
}
