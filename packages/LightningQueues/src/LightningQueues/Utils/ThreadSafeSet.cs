using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FubuCore;

namespace LightningQueues.Utils
{
    public class ThreadSafeSet<T>
	{
		private readonly HashSet<T> inner = new HashSet<T>();
		private readonly ReaderWriterLockSlim rwl = new ReaderWriterLockSlim();

		public void Add(IEnumerable<T> items)
		{
			rwl.EnterWriteLock();
			try
			{
				foreach (var item in items)
				{
					inner.Add(item);
				}
			}
			finally
			{
				rwl.ExitWriteLock();
			}
		}

        public IEnumerable<T> All()
        {
            return rwl.Read(() => all().ToList());
        }

        private IEnumerable<T> all()
        {
            foreach (var item in inner)
            {
                yield return item;
            }
        }

		public IEnumerable<TK> Filter<TK>(IEnumerable<TK> items, Func<TK,T> translator)
		{
			rwl.EnterReadLock();
			try
			{
				foreach (var item in items)
				{
					if (inner.Contains(translator(item)))
						continue;
					yield return item;
				}
			}
			finally
			{
				rwl.ExitReadLock();
			}
		}

		public void Remove(IEnumerable<T> items)
		{
			rwl.EnterWriteLock();
			try
			{
				foreach (var item in items)
				{
					inner.Remove(item);
				}
			}
			finally
			{
				rwl.ExitWriteLock();
			}
		}

        public void Remove(T item)
        {
            rwl.Write(() =>
            {
                inner.Remove(item);
            });
        }
	}
}