using System;
using System.Diagnostics;

namespace FubuMVC.Core.Behaviors
{
    public class StopwatchBehavior : IActionBehavior
    {
        private readonly Action<double> _record;

        public StopwatchBehavior(Action<double> record)
        {
            _record = record;
        }

        // The underlying IoC container would inject the "inner"
        // behavior via this property
        public IActionBehavior InnerBehavior { get; set; }

        public void Invoke()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                InnerBehavior.Invoke();
            }
            finally
            {
                stopwatch.Stop();
                _record(stopwatch.ElapsedMilliseconds);
            }
        }

        public void InvokePartial()
        {
            Invoke();
        }
    }
}