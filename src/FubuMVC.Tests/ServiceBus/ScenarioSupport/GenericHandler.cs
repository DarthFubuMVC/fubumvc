using FubuMVC.Core.ServiceBus.Runtime;

namespace FubuMVC.Tests.ServiceBus.ScenarioSupport
{
    public class GenericHandler : SimpleHandler<Message>
    {
        public GenericHandler(Envelope envelope) : base(envelope)
        {
        }
    }
}