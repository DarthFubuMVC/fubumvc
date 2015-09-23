using FubuMVC.Core.ServiceBus.Runtime;

namespace TestMessages.ScenarioSupport
{
    public class DifferentOneHandler : SimpleHandler<OneMessage>
    {
        public DifferentOneHandler(Envelope envelope) : base(envelope)
        {
        }
    }
}