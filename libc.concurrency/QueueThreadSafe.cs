using System.Collections.Generic;

namespace libc.concurrency
{
    public class QueueThreadSafe<T>
    {
        private readonly object lck;
        private readonly Queue<T> q;

        public QueueThreadSafe()
        {
            q = new Queue<T>();
            lck = new object();
        }

        public int Count
        {
            get
            {
                var cnt = -1;

                lock (lck)
                {
                    try
                    {
                        cnt = q.Count;
                    }
                    catch
                    {
                        // ignored
                    }
                }

                return cnt;
            }
        }

        public Queue<T> Duplicate()
        {
            var res = new Queue<T>();

            lock (lck)
            {
                try
                {
                    foreach (var item in q) res.Enqueue(item);
                }
                catch
                {
                    // ignored
                }
            }

            return res;
        }

        public void Clear()
        {
            lock (lck)
            {
                try
                {
                    q.Clear();
                }
                catch
                {
                    // ignored
                }
            }
        }

        public bool TryDequeue(out T item)
        {
            lock (lck)
            {
                try
                {
                    if (q.Count > 0)
                    {
                        item = q.Dequeue();

                        return true;
                    }
                }
                catch
                {
                    // ignored
                }
            }

            item = default;

            return false;
        }

        public List<T> DequeueAll()
        {
            var res = new List<T>();

            lock (lck)
            {
                try
                {
                    while (q.Count > 0) res.Add(q.Dequeue());
                }
                catch
                {
                    // ignored
                }
            }

            return res;
        }

        public List<T> DequeueMany(int N)
        {
            var res = new List<T>();

            lock (lck)
            {
                try
                {
                    for (var i = 0; i < N; i++)
                    {
                        T b;

                        if (TryDequeue(out b))
                            res.Add(b);
                        else
                            break;
                    }
                }
                catch
                {
                    // ignored
                }
            }

            return res;
        }

        public void Enqueue(T item)
        {
            lock (lck)
            {
                try
                {
                    q.Enqueue(item);
                }
                catch
                {
                    // ignored
                }
            }
        }

        public void EnqueueMany(IEnumerable<T> items)
        {
            lock (lck)
            {
                try
                {
                    foreach (var item in items) q.Enqueue(item);
                }
                catch
                {
                    // ignored
                }
            }
        }

        public bool Any()
        {
            return Count > 0;
        }

        ~QueueThreadSafe()
        {
            Clear();
        }
    }
}