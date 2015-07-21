using FubuMVC.Core.ServiceBus.Runtime;

namespace FubuMVC.Tests.ServiceBus.ScenarioSupport
{
    public class AnotherOneHandler : SimpleHandler<OneMessage>
    {
        public AnotherOneHandler(Envelope envelope) : base(envelope)
        {
        }
    }
}