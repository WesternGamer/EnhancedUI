using System;
using System.Collections.Generic;
using System.Threading;

namespace EnhancedUI.Utils
{
    /// <summary>
    /// Tracker to allow concurrently collect items, then automatically processing the collected ones.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Tracker<T>
    {
        private readonly Mutex mutex = new();
        private readonly HashSet<T> items = new();

        public Context Process()
        {
            return new Context(this);
        }

        public void Add(T item)
        {
            mutex.WaitOne();
            try
            {
                items.Add(item);
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }

        public int Count
        {
            get
            {
                mutex.WaitOne();
                try
                {
                    return items.Count;
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }
        }

        public class Context : IDisposable
        {
            private readonly Tracker<T> tracker;

            public HashSet<T> Items => tracker.items;

            public Context(Tracker<T> tracker)
            {
                this.tracker = tracker;
                tracker.mutex.WaitOne();
            }

            public void Dispose()
            {
                tracker.items.Clear();
                tracker.mutex.ReleaseMutex();
            }
        }
    }
}