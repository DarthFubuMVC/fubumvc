using System.Collections.Generic;

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

        public double Average
        {
            get { return TotalExecutionTime/HitCount; }
        }

        public double ExceptionPercentage
        {
            get { return (((double)ExceptionCount)/((double)HitCount))*100; }
        }

        public bool IsWarning(ChainExecutionLog report)
        {
            var max = MaxTime;
            var avg = Average;
            var p1 = 1 - report.ExecutionTime / max;
            var p2 = 1 - (double)avg / max;
            return (p2 - p1) > 0.25;
        }

        public Dictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>
            {
                {"hits", HitCount},
                {"total", TotalExecutionTime},
                {"average", Average},
                {"exceptions", ExceptionPercentage},
                {"min", MinTime},
                {"max", MaxTime}

            };
        } 

    }
}