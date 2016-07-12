using System.Threading.Tasks;

namespace FubuMVC.Core.ServiceBus.Runtime.Invocation
{
    public abstract class SimpleEnvelopeHandler : IEnvelopeHandler, IContinuation
    {
        public Task<IContinuation> Handle(Envelope envelope)
        {
            return Matches(envelope) ? Task.FromResult<IContinuation>(this) : Task.FromResult<IContinuation>(null);
        }

        public abstract bool Matches(Envelope envelope);

        public abstract void Execute(Envelope envelope, IEnvelopeContext context);
    }
}