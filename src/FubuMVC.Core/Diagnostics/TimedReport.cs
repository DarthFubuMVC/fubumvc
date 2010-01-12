using System.Diagnostics;

namespace FubuMVC.Core.Diagnostics
{
    public class TimedReport
    {
        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
        public double ExecutionTime { get; private set; }

        public void MarkFinished()
        {
            _stopwatch.Stop();

            ExecutionTime = _stopwatch.ElapsedMilliseconds;
        }
    }
}