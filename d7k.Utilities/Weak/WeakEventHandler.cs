using System;
using System.Collections.Generic;
using System.Linq;

namespace d7k.Utilities
{
	/// <summary>
	/// Weak event (don't safe for Garbage Collector)
	/// </summary>
	/// <typeparam name="T">Event argument type</typeparam>
	/// <remarks>
	/// When we subscribe on some event we should memory: construction
	/// 
	/// WeakEventHandler += function
	/// 
	/// is sintacsis shuger and builder extract it to:
	/// 
	/// WeakEventHandler += new EventHandler(function)
	/// 
	/// Since WeakEventHandler don't safe used objects (look at summary)
	/// therefore [new EventHandler] will destroy after first GC work. For fix the behavior
	/// must have use variable (for example _hFunction) when you subscribe on the event:
	/// 
	/// WeakEventHandler += _hFunction = function
	/// 
	/// If you use _hFunction then the variable will block destroy [new EventHandler].
	/// Event object will exists during the variable use event reference.
	/// </remarks>
	public class WeakEventHandler<T> where T : EventArgs
	{
		private List<WeakReference> m_handlers = new List<WeakReference>();

		public static WeakEventHandler<T> operator +
			(WeakEventHandler<T> source, EventHandler<T> handler)
		{
			source.Add(handler);
			return source;
		}

		public static WeakEventHandler<T> operator -
			(WeakEventHandler<T> source, EventHandler<T> handler)
		{
			source.Remove(handler);
			return source;
		}

		public void Add(EventHandler<T> handler)
		{
			m_handlers.Add(new WeakReference(handler));
		}

		public void Remove(EventHandler<T> handler)
		{
			m_handlers.RemoveAll(x => object.ReferenceEquals(handler, x.Target));
		}

		public int Count { get { return m_handlers.Count; } }

		public int AliveCount { get { return m_handlers.Count(x => x.IsAlive); } }

		public void Invoke(object sender, T arg)
		{
			var handlers = (from x in m_handlers select (EventHandler<T>)x.Target).ToArray();

			foreach (var v in handlers)
			{
				if (v != null)
					ExecHandler(sender, arg, v);
				else
					Remove(v);
			}
		}

#if DEBUG
		private static void ExecHandler(object sender, T arg, EventHandler<T> handler)
		{
			try
			{
				handler(sender, arg);
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine(string.Format("WeakEventHandler: an exception of a handler {0}", e));
				throw;
			}
		}
#else
		private static void ExecHandler(object sender, T arg, EventHandler<T> handler)
		{
			handler(sender, arg);
		}
#endif
	}
}
