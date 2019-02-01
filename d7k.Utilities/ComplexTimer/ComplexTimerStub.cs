using System;
using System.Collections.Generic;

namespace d7k.Utilities.Tasks
{
	public class ComplexTimerStub : IComplexTimer
	{
		public IDisposable Start(string name)
		{
			return new DisposeManager();
		}

		public IComplexTimer AddCount(string name, long addCount)
		{
			return this;
		}

		public IComplexTimer SetText(string name, string value)
		{
			return this;
		}

		public long GetCount(string name)
		{
			return 0;
		}
		
		public IComplexTimer SetCount(string name, long count)
		{
			return this;
		}

		public Dictionary<string, object> AllValues()
		{
			return new Dictionary<string, object>();
		}
	}
}
