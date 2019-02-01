using System;
using System.Threading;

namespace d7k.Utilities
{
	public class RwSync
	{
		ReaderWriterLock m_lock = new ReaderWriterLock();
		TimeSpan m_timeout;

		public RwSync(TimeSpan timeout)
		{
			m_timeout = timeout;
		}


		public RwSync()
			: this(TimeSpan.FromMinutes(10))
		{
		}

		public void StartWrite(DisposeManager session)
		{
			try { }
			finally
			{
				//the critical section - finaly block can not be break by an async exception (i.e. ThreadAbortException)
				session.OnDispose += () => m_lock.ReleaseWriterLock();
				m_lock.AcquireWriterLock(m_timeout);
			}
		}

		public void StartRead(DisposeManager session)
		{
			try { }
			finally
			{
				//the critical section - finaly block can not be break by an async exception (i.e. ThreadAbortException)
				session.OnDispose += () => m_lock.ReleaseReaderLock();
				m_lock.AcquireReaderLock(m_timeout);
			}
		}
	}
}
