namespace FubuMVC.Core.ServiceBus.Runtime.Invocation
{
    public interface IEnvelopeHandler
    {
        IContinuation Handle(Envelope envelope);
    }
}