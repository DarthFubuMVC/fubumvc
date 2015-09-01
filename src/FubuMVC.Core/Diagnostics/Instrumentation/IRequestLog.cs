using FubuMVC.Core.ServiceBus.Runtime;

namespace FubuMVC.Core.Diagnostics.Instrumentation
{
    public interface IRequestLog
    {
        double ExecutionTime { get; }
        bool HadException { get; }
    }
}