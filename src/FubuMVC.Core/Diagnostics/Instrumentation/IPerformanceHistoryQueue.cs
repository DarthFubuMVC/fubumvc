namespace FubuMVC.Core.Diagnostics.Instrumentation
{
    public interface IPerformanceHistoryQueue
    {
        void Dispose();
        void Enqueue(ChainExecutionLog log);
        void Start();
    }
}