using FubuMVC.Core.ServiceBus.Runtime;

namespace FubuMVC.Tests.ServiceBus.ScenarioSupport
{
    public class OneHandler : SimpleHandler<OneMessage>
    {
        public OneHandler(Envelope envelope) : base(envelope)
        {
        }
    }
}