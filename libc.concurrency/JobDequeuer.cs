using System;
using System.Collections.Generic;
using System.Linq;

namespace libc.concurrency
{
    /// <summary>
    ///     A job queue. You can add jobs to this instance and jobs will be dequeued every N milliseconds
    /// </summary>
    /// <typeparam name="TJob"></typeparam>
    public class JobDequeuer<TJob> : IDisposable
    {
        private readonly List<TJob> list;
        private readonly DelayedTask timer;
        private bool disposed;

        public JobDequeuer(int sleepMilliseconds)
        {
            list = new List<TJob>();
            timer = new DelayedTask(check, sleepMilliseconds);
            timer.Start();
        }

        public Action<List<TJob>> NewJobs { get; set; }

        public void Dispose()
        {
            disposed = true;
            timer.Stop();
        }

        public void Enqueue(TJob job)
        {
            if (disposed) return;

            lock (list)
            {
                list.Add(job);
            }
        }

        private void check()
        {
            try
            {
                if (disposed) return;
                List<TJob> jobs = null;

                lock (list)
                {
                    if (list.Count > 0)
                    {
                        jobs = list.ToList();
                        list.Clear();
                    }
                }

                if (jobs != null) NewJobs?.Invoke(jobs);
            }
            finally
            {
                if (!disposed) timer.Start();
            }
        }
    }
}