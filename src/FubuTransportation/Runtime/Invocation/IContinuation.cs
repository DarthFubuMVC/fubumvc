namespace FubuTransportation.Runtime.Invocation
{
    public interface IContinuation
    {
        void Execute(Envelope envelope, ContinuationContext context);
    }
}