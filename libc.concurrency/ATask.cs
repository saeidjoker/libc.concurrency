using System;
using System.Timers;
namespace libc.concurrency {
    public class ATask : IDisposable {
        private readonly Action action;
        private readonly Timer timer;
        private double duration;
        public ATask(Action callback, int duration) {
            action = callback;
            this.duration = duration;
            timer = new Timer {
                AutoReset = false
            };
            timer.Elapsed += tmr_Elapsed;
        }
        public double Duration {
            get => duration;
            set {
                duration = value;
                if (duration > 0) timer.Interval = value;
            }
        }
        public void Dispose() {
            Cancel();
            timer?.Dispose();
        }
        public void Execute() {
            if (Math.Abs(Duration) < 0.0009) {
                action.BeginInvoke(null, null);
            } else {
                timer.Interval = Duration;
                timer.Start();
            }
        }
        public void Cancel() {
            try {
                timer.Stop();
            } catch {
                // ignored
            }
        }
        public void Reset() {
            Cancel();
            Execute();
        }
        private void tmr_Elapsed(object sender, ElapsedEventArgs e) {
            action();
        }
    }
}