namespace FubuMVC.Core.Diagnostics.Instrumentation
{
    public class InMemoryExecutionLogStorage : IExecutionLogStorage
    {
        private readonly IChainExecutionHistory _history;
        private readonly IPerformanceHistoryQueue _queue;

        public InMemoryExecutionLogStorage(IChainExecutionHistory history, IPerformanceHistoryQueue queue)
        {
            _history = history;
            _queue = queue;
        }

        public void Store(IChainExecutionLog log)
        {
            _history.Store((ChainExecutionLog) log);
            _queue.Enqueue((ChainExecutionLog) log);
        }
    }
}