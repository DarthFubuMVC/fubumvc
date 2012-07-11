using System;

namespace FubuMVC.Diagnostics.Runtime
{
    public interface IDebugReport
    {
        Guid Id { get; }
        Guid BehaviorId { get; set; }
        string Url { get; set; }
        string HttpMethod { get; }
        DateTime Time { get; set; }

        double ExecutionTime { get; }

        // T Last<T>() where T : class, new()

        void MarkFinished();
        void AddLog(object log);
    }
}