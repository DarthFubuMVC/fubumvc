namespace FubuTransportation.Runtime.Invocation
{
    public interface IEnvelopeHandler
    {
        IContinuation Handle(Envelope envelope);
    }
}