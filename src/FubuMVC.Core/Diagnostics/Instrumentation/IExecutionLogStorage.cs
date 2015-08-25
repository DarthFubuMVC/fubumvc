namespace FubuMVC.Core.Diagnostics.Instrumentation
{
    public interface IExecutionLogStorage
    {
        void Store(IChainExecutionLog log);
    }
}