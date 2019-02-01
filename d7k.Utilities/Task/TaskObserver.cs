using System;

namespace d7k.Utilities.Tasks
{
	public class TaskObserver : IObserver, IDisposable
	{
		public static MessageType<object> Begin { get; private set; }
		public static MessageType<int> NextStep { get; private set; }
		public static MessageType<object> Complete { get; private set; }
		public static MessageType<object> Faild { get; private set; }
		public static MessageType<TimeSpan> Timeout { get; private set; }

		static TaskObserver()
		{
			Begin = new MessageType<object>("Begin");
			NextStep = new MessageType<int>("NextStep");
			Complete = new MessageType<object>("Complete");
			Faild = new MessageType<object>("Faild");
			Timeout = new MessageType<TimeSpan>("Timeout");
		}

		IObserver m_observer;
		bool m_isComplete;
		DateTime m_start;
		int m_stepBlockSize;
		int m_currentStep;
		DisposeManager m_disposer;

		public TaskObserver(IObserver observer, int stepBlockSize, string startMessage)
		{
			m_observer = observer;
			m_isComplete = false;
			m_start = DateTime.Now;
			m_stepBlockSize = stepBlockSize;
			m_currentStep = 0;

			m_disposer = new DisposeManager();
			m_disposer.OnDispose += OnDispose;

			observer.Send(TaskObserver.Begin, null);
			if (startMessage != null)
				observer.Send(MessageTypes.Message, startMessage);
		}

		public TaskObserver(IObserver observer, string startMessage)
			: this(observer, 1, startMessage)
		{
		}

		public TaskObserver(IObserver observer)
			: this(observer, 1, null)
		{
		}

		public TaskObserver(IObserver observer, int stepBlockSize)
			: this(observer, stepBlockSize, null)
		{
		}

		public void Dispose()
		{
			m_disposer.Dispose();
		}

		public void Send<T>(MessageType<T> type, T data)
		{
			if (object.Equals(TaskObserver.NextStep, type))
				OnNextStep(data);
			else if (object.Equals(TaskObserver.Complete, type))
				OnComplete();
			else
				m_observer.Send(type, data);
		}

		void OnNextStep(object data)
		{
			var oldStep = m_currentStep;
			m_currentStep += (int)data;

			if (m_currentStep / m_stepBlockSize > oldStep / m_stepBlockSize)
				m_observer.Send(TaskObserver.NextStep, m_currentStep);
		}

		void OnComplete()
		{
			m_isComplete = true;
			EndSends(TaskObserver.Complete);
		}

		void OnDispose()
		{
			if (!m_isComplete)
				EndSends(TaskObserver.Faild);
		}

		private void EndSends(MessageType<object> result)
		{
			if (m_currentStep != 0 &&
				m_currentStep % m_stepBlockSize != 0)
			{
				m_observer.Send(TaskObserver.NextStep, m_currentStep);
			}

			m_observer.Send(TaskObserver.Timeout, DateTime.Now - m_start);
			m_observer.Send(result, null);
		}
	}

	public static class MessageTypeHelper
	{
		public static void NextStep(this TaskObserver task, int steps)
		{
			task.Send(TaskObserver.NextStep, steps);
		}

		public static void NextStep(this TaskObserver task)
		{
			task.NextStep(1);
		}

		public static void Complete(this TaskObserver task, string text, params object[] pars)
		{
			task.SendMessage(text, pars);
			task.Complete();
		}

		public static void Complete(this TaskObserver task)
		{
			task.Send(TaskObserver.Complete, null);
		}
	}
}
