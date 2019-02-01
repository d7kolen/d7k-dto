using System;
using System.Collections.Generic;

namespace d7k.Utilities
{
	public class DisposeManager : IDisposable
	{
		object m_target;
		List<Action> m_onDispose;

		public event Action OnDispose
		{
			add { m_onDispose.Add(value); }
			remove { throw new NotImplementedException("It's not clear why"); }
		}

		public DisposeManager(object target)
		{
			System.Diagnostics.Debug.Assert(target != null);

			m_onDispose = new List<Action>();
			m_target = target;
		}

		public DisposeManager()
			: this(new object())
		{
		}

		void Execute()
		{
			// обратный порядок связан с тем, 
			// что иерархий классов надо обработать сначало потомков затем предков 
			// (то есть в обратном созданию)
			for (int i = m_onDispose.Count - 1; i >= 0; i--)
				m_onDispose[i]();
		}

		public void Dispose(bool finalize)
		{
			if (m_target == null)
				return;

			if (!finalize)
				GC.SuppressFinalize(m_target);

			Execute();
			m_target = null;
		}

		public void Dispose()
		{
			Dispose(false);
		}

		public T SafeCreate<T>(Func<T> create)
		{
			T res;

			try { }
			finally
			{
				res = create();
			}

			Breakpoint.Define("complete create");
			return res;
		}

		public T SafeInit<T>(Func<T> create) where T : IDisposable
		{
			T res;

			try { }
			finally
			{
				res = create();
				OnDispose += res.Dispose;
			}

			Breakpoint.Define("complete create");
			return res;
		}
	}
}
