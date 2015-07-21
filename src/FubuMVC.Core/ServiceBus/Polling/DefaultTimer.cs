using System;
using System.Timers;

namespace FubuMVC.Core.ServiceBus.Polling
{
    public class DefaultTimer : ITimer
    {
        private readonly Timer _timer;
        private Action _callback;

        public DefaultTimer()
        {
            _timer = new Timer { AutoReset = false };
            _timer.Elapsed += elapsedHandler;
        }

        public bool Enabled { get; private set; }

        public double Interval
        {
            get
            {
                return _timer.Interval;
            }
            set
            {
                _timer.Interval = value;
            }
        }

        public void Start(Action callback, double interval)
        {
            _callback = callback;
            _timer.Interval = interval;

            _timer.Start();
            Enabled = true;
        }

        public void Stop()
        {
            Enabled = false;
            _timer.Stop();
        }

        public void Restart()
        {
            _timer.Start();
            Enabled = true;
        }

        public void Dispose()
        {
            _timer.Dispose();
        }

        private void elapsedHandler(object sender, ElapsedEventArgs eventArgs)
        {
            if (!Enabled) return;
            if (_callback == null) return;

            _callback();

            // Callback could take a while, recheck Enabled
            if (Enabled)
            {
                // TODO -- harden this w/ errors?  Or handle it in the chains?
                _timer.Start();
            }
        }
    }
}