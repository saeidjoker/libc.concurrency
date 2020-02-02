using System;
using System.Collections.Generic;
using System.Linq;
namespace libc.concurrency {
    /// <summary>
    /// A job queue. You can add jobs to this instance and jobs will be dequeued every N milliseconds
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class JobQ<T> : IDisposable {
        private readonly List<T> list;
        private readonly ATask timer;
        private bool disposed;
        public JobQ(int sleepMilliseconds) {
            list = new List<T>();
            timer = new ATask(check, sleepMilliseconds);
            timer.Execute();
        }
        public Action<List<T>> NewJobs { get; set; }
        public void Dispose() {
            disposed = true;
            timer.Cancel();
        }
        public void Add(T job) {
            if (disposed) return;
            lock (list) {
                list.Add(job);
            }
        }
        private void check() {
            try {
                if (disposed) return;
                List<T> jobs = null;
                lock (list) {
                    if (list.Count > 0) {
                        jobs = list.ToList();
                        list.Clear();
                    }
                }
                if (jobs != null) NewJobs?.Invoke(jobs);
            } finally {
                if (!disposed) timer.Execute();
            }
        }
    }
}