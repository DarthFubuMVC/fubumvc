using FubuMVC.Core.ServiceBus.Runtime;

namespace FubuMVC.Tests.ServiceBus.ScenarioSupport
{
    public class TwoHandler : SimpleHandler<TwoMessage>
    {
        public TwoHandler(Envelope envelope) : base(envelope)
        {
        }
    }
}