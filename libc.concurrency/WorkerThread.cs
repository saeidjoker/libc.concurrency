using System;
namespace libc.concurrency {
    public class WorkerThread {
        private readonly Action job;
        private readonly ATask task;
        public WorkerThread(Action action, int sleepTime) {
            job = action;
            task = new ATask(main, sleepTime);
        }
        public bool IsRunning { get; private set; }
        public void Start() {
            IsRunning = true;
            task.Execute();
        }
        public void Stop() {
            IsRunning = false;
            task.Cancel();
        }
        private void main() {
            try {
                job();
            } catch {
                // ignored
            }
            if (IsRunning) task.Execute();
        }
    }
}