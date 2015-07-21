namespace FubuMVC.Core.ServiceBus.Runtime.Invocation
{
    public interface IContinuation
    {
        void Execute(Envelope envelope, ContinuationContext context);
    }
}