namespace FubuMVC.Core.Diagnostics.Instrumentation
{
    public class PerformanceHistory
    {
        public void Read(IRequestLog log)
        {
            HitCount++;
            if (log.HadException) ExceptionCount++;
            TotalExecutionTime += log.ExecutionTime;

            if (log.ExecutionTime > MaxTime) MaxTime = log.ExecutionTime;
            if (log.ExecutionTime < MinTime || MinTime == 0) MinTime = log.ExecutionTime;
        }

        public long HitCount { get; private set; }
        public long ExceptionCount { get; private set; }

        public double TotalExecutionTime { get; private set; }

        public double MaxTime { get; private set; }
        public double MinTime { get; private set; }

    }
}