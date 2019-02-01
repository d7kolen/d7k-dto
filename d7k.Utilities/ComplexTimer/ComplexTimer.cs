using System;
using System.Collections.Generic;

namespace d7k.Utilities.Tasks
{
	public class ComplexTimer : IComplexTimer
	{
		object m_lock = new object();

		Dictionary<string, string> m_names = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
		Dictionary<string, long> m_counter = new Dictionary<string, long>(StringComparer.InvariantCultureIgnoreCase);
		Dictionary<string, DateTime> m_lastCounter = new Dictionary<string, DateTime>(StringComparer.InvariantCultureIgnoreCase);
		Dictionary<string, long> m_times = new Dictionary<string, long>(StringComparer.InvariantCultureIgnoreCase);
		Dictionary<Guid, TimeMarker> m_openTimes = new Dictionary<Guid, TimeMarker>();
		
		public void ReadAllCounters(ComplexTimer srcTimer, string prefix)
		{
			lock (m_lock)
				lock (srcTimer.m_lock)
				{
					foreach (var t in srcTimer.m_names)
						m_names[prefix + t.Key] = t.Value;
					foreach (var t in srcTimer.m_counter)
						m_counter[prefix + t.Key] = t.Value;
					foreach (var t in srcTimer.m_lastCounter)
						m_lastCounter[prefix + t.Key] = t.Value;
					foreach (var t in srcTimer.m_times)
						m_times[prefix + t.Key] = t.Value;
				}
		}

		public IDisposable Start(string name)
		{
			lock (m_lock)
			{
				Guid key = Guid.NewGuid();

				if (!m_times.ContainsKey(name))
					m_times[name] = 0;

				AddCount(name, 1);
				var now = System.Diagnostics.Stopwatch.GetTimestamp();
				m_openTimes[key] = new TimeMarker(name, now);

				var disp = new DisposeManager();
				disp.OnDispose += () =>
				{
					lock (m_lock)
					{
						m_times[name] = System.Diagnostics.Stopwatch.GetTimestamp() - now + m_times[name];
						m_openTimes.Remove(key);
					}
				};

				return disp;
			}
		}

		public IComplexTimer AddCount(string name, long addCount)
		{
			lock (m_lock)
			{
				if (!m_counter.ContainsKey(name))
					m_counter[name] = 0;

				m_counter[name] += addCount;
				m_lastCounter[name] = DateTime.UtcNow;
				return this;
			}
		}

		public IComplexTimer SetCount(string name, long count)
		{
			lock (m_lock)
			{
				if (!m_counter.ContainsKey(name))
					m_counter[name] = 0;

				m_counter[name] = count;
				m_lastCounter[name] = DateTime.UtcNow;
				return this;
			}
		}

		public IComplexTimer SetText(string name, string value)
		{
			m_names[name] = value;
			return this;
		}

		public long GetCount(string name)
		{
			return m_counter.ContainsKey(name) ? m_counter[name] : 0;
		}
		
		public Dictionary<string, object> AllValues()
		{
			lock (m_lock)
			{
				var now = System.Diagnostics.Stopwatch.GetTimestamp();
				var timeNow = DateTime.UtcNow;
				var opens = new Dictionary<string, long>();
				foreach (var t in m_openTimes.Values)
				{
					if (!opens.ContainsKey(t.Name))
						opens[t.Name] = 0;
					opens[t.Name] += now - t.Stopwatch;
				}

				var result = new Dictionary<string, object>();

				foreach (var t in m_names)
					result[t.Key] = t.Value;

				foreach (var t in m_times)
					if (!result.ContainsKey(t.Key))
					{
						if (opens.ContainsKey(t.Key))
						{
							var ticks = t.Value + opens[t.Key];
							result[t.Key] = TimeSpanForFrequency(ticks);
						}
						else
						{
							var ticks = t.Value;
							result[t.Key] = TimeSpanForFrequency(ticks);
						}

						result[t.Key + ".Count"] = m_counter[t.Key];
					}

				foreach (var t in m_counter)
					if (!result.ContainsKey(t.Key))
					{
						result[t.Key] = m_counter[t.Key];
						result[t.Key + ".Last"] = timeNow - m_lastCounter[t.Key];
					}

				return result;
			}
		}		

		static TimeSpan TimeSpanForFrequency(long value)
		{
			return TimeSpan.FromTicks(value);
		}		
	}

	public static class ComplexDurationHelper
	{
		public static IEnumerable<T> Duration<T>(this IEnumerable<T> items, IComplexTimer duration, string name)
		{
			using (duration.Start(name))
				foreach (var t in items)
					yield return t;
		}
	}

	class TimeMarker
	{
		public string Name { get; set; }
		public long Stopwatch { get; set; }

		public TimeMarker(string name, long stopwatchMark)
		{
			Name = name;
			Stopwatch = stopwatchMark;
		}
	}
}
