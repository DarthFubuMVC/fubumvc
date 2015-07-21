using FubuMVC.Core.ServiceBus.Runtime;

namespace FubuMVC.Tests.ServiceBus.ScenarioSupport
{
    public class ThreeHandler : SimpleHandler<ThreeMessage>
    {
        public ThreeHandler(Envelope envelope) : base(envelope)
        {
        }
    }
}