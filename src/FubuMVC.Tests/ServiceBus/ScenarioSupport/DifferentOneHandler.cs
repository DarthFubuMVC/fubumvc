using FubuMVC.Core.ServiceBus.Runtime;

namespace FubuMVC.Tests.ServiceBus.ScenarioSupport
{
    public class DifferentOneHandler : SimpleHandler<OneMessage>
    {
        public DifferentOneHandler(Envelope envelope) : base(envelope)
        {
        }
    }
}