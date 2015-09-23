using FubuMVC.Core.ServiceBus.Runtime;

namespace TestMessages.ScenarioSupport
{
    public class FourHandler : SimpleHandler<FourMessage>
    {
        public FourHandler(Envelope envelope) : base(envelope)
        {
        }
    }
}