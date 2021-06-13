using System;
using System.Timers;

namespace libc.concurrency
{
    /// <summary>
    ///     Provides Debounce() and Throttle() methods.
    ///     Use these methods to ensure that events aren't handled too frequently.
    ///     Throttle() ensures that events are throttled by the interval specified.
    ///     Only the last event in the interval sequence of events fires.
    ///     Debounce() fires an event only after the specified interval has passed
    ///     in which no other pending event has fired. Only the last event in the
    ///     sequence is fired.
    /// </summary>
    public class Debounce
    {
        private Timer timer;
        private DateTime timerStarted { get; set; } = DateTime.UtcNow.AddYears(-1);

        /// <summary>
        ///     Debounce an event by resetting the event timeout every time the event is
        ///     fired. The behavior is that the Action passed is fired only after events
        ///     stop firing for the given timeout period.
        ///     Use Debounce when you want events to fire only after events stop firing
        ///     after the given interval timeout period.
        ///     Wrap the logic you would normally use in your event code into
        ///     the  Action you pass to this method to debounce the event.
        ///     Example: https://gist.github.com/RickStrahl/0519b678f3294e27891f4d4f0608519a
        /// </summary>
        /// <param name="interval">Timeout in Milliseconds</param>
        /// <param name="action">Action<object> to fire when debounced event fires</object></param>
        /// <param name="param">optional parameter</param>
        public void Invoke(int interval, Action<object> action, object param = null)
        {
            // kill pending timer and pending ticks
            timer?.Stop();
            timer = null;

            // timer is recreated for each event and effectively
            // resets the timeout. Action only fires after timeout has fully
            // elapsed without other events firing in between
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
        }
    }
}