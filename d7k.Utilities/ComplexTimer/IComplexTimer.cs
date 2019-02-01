using System;
using System.Collections.Generic;

namespace d7k.Utilities.Tasks
{
	public interface IComplexTimer
	{
		IDisposable Start(string name);
		IComplexTimer AddCount(string name, long addCount);
		IComplexTimer SetText(string name, string value);
		IComplexTimer SetCount(string name, long count);
		long GetCount(string name);
		Dictionary<string, object> AllValues();
	}
}