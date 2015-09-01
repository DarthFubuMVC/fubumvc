namespace FubuMVC.Core.ServiceBus.Runtime.Invocation
{
    public abstract class SimpleEnvelopeHandler : IEnvelopeHandler, IContinuation
    {
        public IContinuation Handle(Envelope envelope)
        {
            return Matches(envelope) ? this : null;
        }

        public abstract bool Matches(Envelope envelope);

        public abstract void Execute(Envelope envelope, IEnvelopeContext context);
    }
}