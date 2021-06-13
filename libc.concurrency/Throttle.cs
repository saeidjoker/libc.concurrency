using System;
using System.Timers;

namespace libc.concurrency
{
    public sealed class Throttle
    {
        private Timer timer;
        private DateTime timerStarted { get; set; } = DateTime.UtcNow.AddYears(-1);

        /// <summary>
        ///     This method throttles events by allowing only 1 event to fire for the given
        ///     timeout period. Only the last event fired is handled - all others are ignored.
        ///     Throttle will fire events every timeout ms even if additional events are pending.
        ///     Use Throttle where you need to ensure that events fire at given intervals.
        /// </summary>
        /// <param name="interval">Timeout in Milliseconds</param>
        /// <param name="action">Action<object> to fire when debounced event fires</object></param>
        /// <param name="param">optional parameter</param>
        public void Invoke(int interval, Action<object> action, object param = null)
        {
            // kill pending timer and pending ticks
            timer?.Stop();
            timer = null;
            var curTime = DateTime.UtcNow;

            // if timeout is not up yet - adjust timeout to fire 
            // with potentially new Action parameters           
            if (curTime.Subtract(timerStarted).TotalMilliseconds < interval)
                interval -= (int) curTime.Subtract(timerStarted).TotalMilliseconds;

            timer = new Timer(interval);

            timer.Elapsed += (sender, args) =>
            {
                if (timer == null)
                    return;

                timer?.Stop();
                timer = null;
                action.Invoke(param);
            };

            timer.Start();
            timerStarted = curTime;
        }
    }
}