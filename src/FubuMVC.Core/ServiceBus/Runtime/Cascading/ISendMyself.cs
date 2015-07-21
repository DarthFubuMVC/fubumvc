namespace FubuMVC.Core.ServiceBus.Runtime.Cascading
{
    public interface ISendMyself
    {
        Envelope CreateEnvelope(Envelope original);
    }
}