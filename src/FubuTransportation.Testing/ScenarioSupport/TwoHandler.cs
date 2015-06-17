using FubuTransportation.Runtime;

namespace FubuTransportation.Testing.ScenarioSupport
{
    public class TwoHandler : SimpleHandler<TwoMessage>
    {
        public TwoHandler(Envelope envelope) : base(envelope)
        {
        }
    }
}