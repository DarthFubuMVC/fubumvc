using System;
using System.Collections.Generic;

namespace FubuMVC.Core.Diagnostics.Instrumentation
{
    public class PerformanceHistory
    {
        public void Read(IRequestLog log)
        {
            LastExecution = log as ChainExecutionLog;

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
            get
            {
                if (HitCount == 0) return 0;
                return TotalExecutionTime/HitCount;
            }
        }

        public double ExceptionPercentage
        {
            get
            {
                if (HitCount == 0) return 0;
                
                return (((double)ExceptionCount)/((double)HitCount))*100;
            }
        }

        public ChainExecutionLog LastExecution { get; private set; }

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
                {"total", TotalExecutionTime.ToString("N")},
                {"average", Average.ToString("N")},
                {"exceptions", ExceptionPercentage.ToString("N")},
                {"min", MinTime.ToString("N")},
                {"max", MaxTime.ToString("N")}

            };
        }

        public override string ToString()
        {
            return string.Format("HitCount: {0}, ExceptionCount: {1}, TotalExecutionTime: {2}, MaxTime: {3}, MinTime: {4}, Average: {5}, ExceptionPercentage: {6}", HitCount, ExceptionCount, TotalExecutionTime, MaxTime, MinTime, Average, ExceptionPercentage);
        }
    }
}