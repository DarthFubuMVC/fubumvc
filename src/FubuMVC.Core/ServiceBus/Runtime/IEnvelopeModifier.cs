namespace FubuMVC.Core.ServiceBus.Runtime
{
    public interface IEnvelopeModifier
    {
        void Modify(Envelope envelope);
    }
}