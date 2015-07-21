using FubuMVC.Core.ServiceBus.Runtime;

namespace FubuMVC.Tests.ServiceBus.ScenarioSupport
{
    public class FourHandler : SimpleHandler<FourMessage>
    {
        public FourHandler(Envelope envelope) : base(envelope)
        {
        }
    }
}