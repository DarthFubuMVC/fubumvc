using System.Threading.Tasks;

namespace FubuMVC.Core.ServiceBus.Runtime.Invocation
{
    public interface IEnvelopeHandler
    {
        Task<IContinuation> Handle(Envelope envelope);
    }
}