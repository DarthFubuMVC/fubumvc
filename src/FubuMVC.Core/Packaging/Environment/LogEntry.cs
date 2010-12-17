using System;

namespace FubuMVC.Core.Packaging.Environment
{
    [Serializable]
    public class LogEntry
    {
        public bool Success { get; set; }
        public string TraceText { get; set; }
        public string Description { get; set; }
        public long TimeInMilliseconds { get; set; }

        public static LogEntry FromPackageLog(object target, PackageRegistryLog log)
        {
            return new LogEntry(){
                Description = target.ToString(),
                Success = log.Success,
                TraceText = log.FullTraceText().Trim(),
                TimeInMilliseconds = log.TimeInMilliseconds
            };
        }

    }
}