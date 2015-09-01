namespace FubuMVC.Core.ServiceBus.Runtime.Invocation
{
    public interface IContinuation
    {
        void Execute(Envelope envelope, IEnvelopeContext context);
    }
}