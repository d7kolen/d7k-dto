using System;
using System.Collections.Generic;

namespace d7k.Utilities.Tasks
{
	public class ProxyComplexTimer : IComplexTimer
	{
		public IComplexTimer Source { get; set; }

		public IComplexTimer AddCount(string name, long addCount)
		{
			return Source.AddCount(name, addCount);
		}

		public long GetCount(string name)
		{
			return Source.GetCount(name);
		}
		
		public IComplexTimer SetCount(string name, long count)
		{
			return Source.SetCount(name, count);
		}

		public IComplexTimer SetText(string name, string value)
		{
			return Source.SetText(name, value);
		}

		public IDisposable Start(string name)
		{
			return Source.Start(name);
		}

		public Dictionary<string, object> AllValues()
		{
			return Source.AllValues();
		}
	}
}
