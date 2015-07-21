namespace FubuMVC.Core.ServiceBus.Runtime
{
    public interface IEnvelopeSender
    {
        string Send(Envelope envelope);
    }
}