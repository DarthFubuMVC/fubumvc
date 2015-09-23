using FubuMVC.Core.ServiceBus.Runtime;

namespace TestMessages.ScenarioSupport
{
    public class GenericHandler : SimpleHandler<Message>
    {
        public GenericHandler(Envelope envelope) : base(envelope)
        {
        }
    }
}