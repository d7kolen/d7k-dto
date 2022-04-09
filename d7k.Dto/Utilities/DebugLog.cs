using System;

namespace d7k.Dto.Utilities
{
	interface IDebugLog
	{
		void Notify(string message);
	}

	class DebugLogStub : IDebugLog
	{
		public void Notify(string message)
		{
		}
	}

	class DebugLogConsole : IDebugLog
	{
		string m_prefix;

		public DebugLogConsole(string prefix)
		{
			m_prefix = prefix;
		}

		public void Notify(string message)
		{
			var fullMessage = $"{m_prefix}: {message}";

			Console.WriteLine(fullMessage);
			System.Diagnostics.Debug.WriteLine(fullMessage);
		}
	}
}