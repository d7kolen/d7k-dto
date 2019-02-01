using System;
using System.Collections.Generic;

namespace d7k.Utilities.Tasks
{
	public class SafeObserver : IObserver
	{
		IObserver m_source;

		public SafeObserver(IObserver source)
		{
			m_source = source;
		}

		public void Send<T>(MessageType<T> type, T data)
		{
			if (data != null)
				try
				{
					m_source.Send(type, data);
				}
				catch (Exception)
				{
					return;
				}
		}
	}

	public class CompositeObserver : IObserver
	{
		public IEnumerable<IObserver> Source { get; private set; }

		public CompositeObserver(IEnumerable<IObserver> source)
		{
			Source = source;
		}

		public void Send<T>(MessageType<T> type, T data)
		{
			foreach (var t in Source)
				t.Send(type, data);
		}
	}

	public class SwitchObserver : IObserver
	{
		Dictionary<MessageType, List<Action<MessageType, object>>> m_dispetch;
		IObserver m_defaultObserver;

		public SwitchObserver(IObserver defaultObserver)
		{
			m_defaultObserver = defaultObserver;
			m_dispetch = new Dictionary<MessageType, List<Action<MessageType, object>>>();
		}

		public SwitchObserver Regist<T>(MessageType<T> type, params Action<MessageType<T>, T>[] senderList)
		{
			List<Action<MessageType, object>> it;
			if (!m_dispetch.TryGetValue(type, out it))
				m_dispetch[type] = it = new List<Action<MessageType, object>>();

			foreach (var sender in senderList)
			{
				var curSender = sender;
				it.Add((t, obj) => curSender((MessageType<T>)type, (T)obj));
			}

			return this;
		}

		public void Send<T>(MessageType<T> type, T data)
		{
			List<Action<MessageType, object>> senderList;

			if (m_dispetch.TryGetValue(type, out senderList))
				foreach (var sender in senderList)
					sender(type, data);
			else
				m_defaultObserver.Send(type, data);
		}
	}

	public class StubObserver : IObserver
	{
		public void Send<T>(MessageType<T> type, T data) { }
	}

	public class CounterObserver : SwitchObserver, IDisposable
	{
		public const string c_countPattern = "{count}";
		static MessageType<int> m_nextStep = new MessageType<int>("next step");

		IObserver m_src;
		public MessageType<int> Msg { get; private set; }
		DisposeManager m_disposer;
		public int Current { get; private set; }
		int m_blockSize;

		public CounterObserver(IObserver src, MessageType<int> countMsg, int blockSize)
			: base(src)
		{
			m_src = src;
			m_blockSize = blockSize;
			Current = 0;
			Msg = countMsg;

			m_disposer = new DisposeManager();
			m_disposer.OnDispose += Complete;

			Regist(m_nextStep, Next);
		}

		public CounterObserver(IObserver src, string pattern, int blockSize)
			: this(src, new MessageType<int>(pattern, x => pattern.Replace(c_countPattern, x.ToString())), blockSize)
		{
		}

		public void Next(int step)
		{
			Send(m_nextStep, step);
		}

		void Next(MessageType<int> type, int step)
		{
			var oldStep = Current;
			Current += step;

			if (Current / m_blockSize > oldStep / m_blockSize)
				m_src.Send(Msg, Current);
		}

		void Complete()
		{
			if (Current != 0 && Current % m_blockSize != 0)
				m_src.Send(Msg, Current);
		}

		public void Dispose()
		{
			m_disposer.Dispose();
		}

		public static CounterObserver operator ++(CounterObserver observer)
		{
			observer.Next(1);
			return observer;
		}
	}
}
