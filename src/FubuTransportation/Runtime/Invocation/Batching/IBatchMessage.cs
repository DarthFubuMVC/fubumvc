namespace FubuTransportation.Runtime.Invocation.Batching
{
    public interface IBatchMessage
    {
        object[] Messages { get; }
    }
}