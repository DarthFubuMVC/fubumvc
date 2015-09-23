using FubuMVC.Core.ServiceBus.Runtime;

namespace TestMessages.ScenarioSupport
{
    public class AnotherOneHandler : SimpleHandler<OneMessage>
    {
        public AnotherOneHandler(Envelope envelope) : base(envelope)
        {
        }
    }
}