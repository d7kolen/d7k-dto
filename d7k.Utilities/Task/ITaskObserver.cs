using System;

namespace d7k.Utilities.Tasks
{
	[Obsolete]
	public interface ITaskObserver
	{
		long CurrentStep { get; set; }

		void NewTask(long maxStep, string desc);

		void WriteLog(object msg);
	}

	[Obsolete]
	public static class TaskObserverHelper
	{
		public static void NextStep(this ITaskObserver observer)
		{
			observer.CurrentStep++;
		}

		public static void WriteLog(this ITaskObserver observer, string logMessage, params object[] args)
		{
			observer.WriteLog(string.Format(logMessage, args));
		}

		public static void NewLine(this ITaskObserver observer)
		{
			observer.WriteLog(String.Empty);
		}

		public static IDisposable LazyWriteLog(this ITaskObserver observer, string logMessage, params object[] args)
		{
			var disp = new DisposeManager();
			disp.OnDispose += () => observer.WriteLog(logMessage, args);
			return disp;
		}
	}

	[Obsolete]
	public class TaskObserverStub : ITaskObserver
	{
		public long CurrentStep { get { return 0; } set { } }

		public void NewTask(long maxStep, string desc) { }

		public void WriteLog(object msg) { }
	}
}
