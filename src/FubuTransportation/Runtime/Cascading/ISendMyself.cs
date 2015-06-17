namespace FubuTransportation.Runtime.Cascading
{
    public interface ISendMyself
    {
        Envelope CreateEnvelope(Envelope original);
    }
}