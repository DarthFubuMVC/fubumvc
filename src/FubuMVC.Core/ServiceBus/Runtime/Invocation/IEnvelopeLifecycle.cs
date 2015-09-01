namespace FubuMVC.Core.ServiceBus.Runtime.Invocation
{
    public interface IEnvelopeLifecycle
    {
        IEnvelopeContext StartNew(IHandlerPipeline pipeline, Envelope envelope);
    }
}