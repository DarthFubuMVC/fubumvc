using FubuMVC.Core.ServiceBus.Runtime;

namespace TestMessages.ScenarioSupport
{
    public class ThreeHandler : SimpleHandler<ThreeMessage>
    {
        public ThreeHandler(Envelope envelope) : base(envelope)
        {
        }
    }
}