using System;
using System.Diagnostics;
using System.IO;

namespace d7k.Utilities.Tasks
{
	public class PrimitiveObservers
	{
		/// <summary>
		/// The Using example in a TPDTask class:
		/// 
		/// public override void Configure(System.Xml.XmlNode rootNode)
		///	{
		///		base.Configure(rootNode);
		///		m_log = PrimitiveObservers.TpdTaskLog(WriteToDBEventLog, "[The Best TPD Task]");
		///	}
		/// </summary>
		public static IObserver TpdTaskLog(Action<string, EventLogEntryType> sendMsg, string taskName = null)
		{
			return new TpdTaskObserver(sendMsg, taskName);
		}

		public static IObserver ConsoleAndFileLog(FileInfo file)
		{
			var dstFile = CheckOldFileLog(file);

			return
				new SafeObserver(
					new TimedObserver(
						new CompositeObserver(
							new IObserver[] { 
								new FileObserver(dstFile), 
								new WriterObserver(Console.Out) })));
		}

		static FileInfo CheckOldFileLog(FileInfo file)
		{
			if (file.Exists)
			{
				var moveName = file.NameOnly() + DateTime.UtcNow.ToString("yyyy-M-d_HH-mm-ss") + file.Extension;
				var currentName = file.FullName;
				file.MoveTo(file.Directory.SubFile(moveName).FullName);

				return new FileInfo(currentName);
			}

			return file;
		}
	}

	class TimedObserver : IObserver
	{
		IObserver m_logTarget;

		public TimedObserver(IObserver logTarget)
		{
			m_logTarget = logTarget;
		}

		public void Send<T>(MessageType<T> type, T data)
		{
			var message = type.ToString(data);
			var fullMessage = DateTime.Now.ToString("hh:mm:ss") + " " + message;

			m_logTarget.Send(MessageTypes.Message, fullMessage);
		}
	}

	public class TpdTaskObserver : IObserver
	{
		Action<string, EventLogEntryType> m_sendMsg;
		string m_prefix;

		public TpdTaskObserver(Action<string, EventLogEntryType> sendMsg, string prefix)
		{
			m_sendMsg = sendMsg;

			if (string.IsNullOrEmpty(prefix))
				m_prefix = "";
			else
				m_prefix = " " + prefix;
		}

		public void Send<T>(MessageType<T> type, T data)
		{
			if (m_sendMsg == null)
				return;

			var message = type.ToString(data);
			var consoleMessage = string.Format("{0}{1}:\t{2}", DateTime.Now.ToString("hh:mm:ss"), m_prefix, message);

			if (type.Equals(MessageTypes.Error))
			{
				m_sendMsg(message, EventLogEntryType.Error);
				Console.WriteLine(consoleMessage);
			}
			else if (type.Equals(MessageTypes.Warning))
			{
				m_sendMsg(message, EventLogEntryType.Warning);
				Console.WriteLine(consoleMessage);
			}
			else
			{
				m_sendMsg(message, EventLogEntryType.Information);
				Console.WriteLine(consoleMessage);
			}
		}
	}
}
