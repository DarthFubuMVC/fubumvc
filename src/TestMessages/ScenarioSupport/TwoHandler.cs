using FubuMVC.Core.ServiceBus.Runtime;

namespace TestMessages.ScenarioSupport
{
    public class TwoHandler : SimpleHandler<TwoMessage>
    {
        public TwoHandler(Envelope envelope) : base(envelope)
        {
        }
    }
}