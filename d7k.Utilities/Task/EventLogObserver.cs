using System;
using System.Diagnostics;

namespace d7k.Utilities.Tasks
{
	public class EventLogObserver : IObserver
	{
		Action<string, EventLogEntryType> m_writeLog;

		public EventLogObserver(Action<string, EventLogEntryType> writeLog)
		{
			m_writeLog = writeLog;
		}

		public void Send<T>(MessageType<T> type, T data)
		{
			var str = type.ToString(data);
			if (MessageTypes.Message.Equals(type))
				m_writeLog(str, EventLogEntryType.Information);
			else if (MessageTypes.Warning.Equals(type))
				m_writeLog(str, EventLogEntryType.Warning);
			else if (MessageTypes.Error.Equals(type))
				m_writeLog(str, EventLogEntryType.Error);
		}
	}
}
