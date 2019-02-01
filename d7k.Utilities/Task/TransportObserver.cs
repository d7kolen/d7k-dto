using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;

namespace d7k.Utilities.Tasks
{
	public class FileObserver : IObserver
	{
		public FileInfo File { get; private set; }

		public FileObserver(FileInfo file)
		{
			File = file;
		}

		public void Send<T>(MessageType<T> type, T data)
		{
			using (var writer = File.AppendText())
				writer.WriteLine(type.ToString(data));
		}
	}

	public class MailObserver : IObserver
	{
		Action<MailMessage> m_send;
		public string Subject { get; set; }
		public string From { get; set; }
		public IEnumerable<string> To { get; set; }

		public MailObserver(SmtpClient smtp)
			: this(x => smtp.Send(x))
		{
		}

		public MailObserver(Action<MailMessage> send)
		{
			m_send = send;
		}

		public void Send<T>(MessageType<T> type, T data)
		{
			var msg = new MailMessage();

			if (Subject != null)
				msg.Subject = Subject;

			if (To != null)
				foreach (var t in To)
					msg.To.Add(t);

			if (From != null)
				msg.From = new MailAddress(From);

			msg.Body = type.ToString(data);
			msg.IsBodyHtml = true;

			m_send(msg);
		}
	}

	public class WriterObserver : IObserver
	{
		public TextWriter Writer { get; private set; }

		public WriterObserver(TextWriter writer)
		{
			Writer = writer;
		}

		public void Send<T>(MessageType<T> type, T data)
		{
			Writer.WriteLine(type.ToString(data));
			Writer.Flush();
		}
	}
}
