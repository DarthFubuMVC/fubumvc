using System;
using System.Diagnostics;

namespace FubuMVC.Core.Behaviors
{
    /// <summary>
    /// Honestly, this is in here as a demonstration more than anything
    /// useful
    /// </summary>
    public class StopwatchBehavior : WrappingBehavior
    {
        private readonly Action<double> _record;

        public StopwatchBehavior(Action<double> record)
        {
            _record = record;
        }

        protected override void invoke(Action action)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                action();
            }
            finally
            {
                stopwatch.Stop();
                _record(stopwatch.ElapsedMilliseconds);
            }
        }
    }
}