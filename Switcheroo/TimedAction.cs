using System;
using System.Windows.Threading;

namespace Switcheroo
{
    public static class TimedAction
    {
        public static void ExecuteWithDelay(Action action, TimeSpan delay)
        {
            var timer = new DispatcherTimer
            {
                Interval = delay,
                Tag = action
            };

            timer.Tick += TimerOnTick;
            timer.Start();
        }

        private static void TimerOnTick(object sender, EventArgs e)
        {
            var timer = (DispatcherTimer) sender;
            var action = (Action) timer.Tag;

            action.Invoke();
            timer.Stop();
        }
    }
}
