namespace FubuMVC.Core.ServiceBus.Runtime.Invocation.Batching
{
    public interface IBatchMessage
    {
        object[] Messages { get; }
    }
}