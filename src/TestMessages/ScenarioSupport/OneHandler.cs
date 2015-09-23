using FubuMVC.Core.ServiceBus.Runtime;

namespace TestMessages.ScenarioSupport
{
    public class OneHandler : SimpleHandler<OneMessage>
    {
        public OneHandler(Envelope envelope) : base(envelope)
        {
        }
    }
}