using System;
using System.Linq;
using System.Text;

namespace d7k.Utilities.Tasks
{
	public interface IObserver
	{
		void Send<T>(MessageType<T> type, T data);
	}

	public class MessageType
	{
		string m_name;

		public MessageType(string name)
		{
			m_name = name;
		}

		public override string ToString()
		{
			return m_name;
		}
	}

	public class MessageType<T> : MessageType
	{
		Func<T, string> m_toString;

		public MessageType(string name)
			: base(name)
		{
			m_toString = x => x.ToString();
		}

		public MessageType(string name, Func<T, string> toString)
			: base(name)
		{
			m_toString = toString;
		}

		public string ToString(T data)
		{
			return m_toString(data);
		}
	}

	public static class MessageTypes
	{
		public static MessageType<string> Message { get; private set; }
		public static MessageType<string> Warning { get; private set; }
		public static MessageType<Exception> Error { get; private set; }

		static MessageTypes()
		{
			Message = new MessageType<string>("Message");
			Error = new MessageType<Exception>("Error");
			Warning = new MessageType<string>("Warning");
		}

		public static void SendMessage(this IObserver observer, string text, params object[] pars)
		{
			observer.Send(MessageTypes.Message, text, pars);
		}

		public static void SendWarning(this IObserver observer, string text, params object[] pars)
		{
			observer.Send(MessageTypes.Warning, text, pars);
		}

		public static void SendError(this IObserver observer, Exception exc)
		{
			observer.Send(MessageTypes.Error, exc);
		}

		public static void SendError(this IObserver observer, string text, params object[] pars)
		{
			observer.Send(MessageTypes.Error, new MessageTypeStubException(string.Format(text, pars)));
		}

		public static void Send(this IObserver observer, MessageType<string> type, string text, params object[] pars)
		{
			string resStr;

			try
			{
				resStr = string.Format(text, pars);
			}
			catch (FormatException)
			{
				resStr = text + "[" + pars.Aggregate(new StringBuilder(), (acc, x) => acc.Append("\t").Append(x)) + "]";
			}

			observer.Send(type, resStr);
		}

		public static IObserver ToSafe(this IObserver source)
		{
			return new SafeObserver(source);
		}
	}

	class MessageTypeStubException : Exception
	{
		public MessageTypeStubException(string message)
			: base(message)
		{
		}
	}
}
