namespace FubuMVC.Core.ServiceBus.Runtime
{
    public abstract class EnvelopeModifier<T> : IEnvelopeModifier where T : class
    {
        public void Modify(Envelope envelope)
        {
            var target = envelope.Message as T;
            if (target != null)
            {
                Modify(envelope, target);
            }
        }

        public abstract void Modify(Envelope envelope, T target);
    }
}