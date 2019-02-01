using System;
using System.Diagnostics;

namespace d7k.Utilities
{
	public class Breakpoint
	{
		private static Action<string> _break = null;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="func"></param>
		/// [Conditional("DEBUG")] - the mechanism should work with DEBUG testing assembly and release library
		public static void SetBreakFunc(Action<string> func)
		{
			_break = func;
		}

		/// <summary>
		/// Set Breakpoints: {name}|before, {name} 
		/// </summary>
		[Conditional("DEBUG")]
		public static void Define(string name)
		{
			if (_break != null)
			{
				_break(name + "|before");
				_break(name);
			}
		}

		/// <summary>
		/// Set Breakpoints: {name}|before, {name}, {name}|{uniqueId}|before, {name}|{uniqueId}
		/// </summary>
		[Conditional("DEBUG")]
		public static void Define(string name, object uniqueId)
		{
			Define(name);
			Define(name + "|" + uniqueId);
		}
	}
}
