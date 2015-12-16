namespace FubuMVC.Core.Diagnostics.Instrumentation
{
    public interface IPerformanceHistoryQueue
    {
        void Enqueue(ChainExecutionLog log);
    }
}