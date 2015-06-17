using FubuTransportation.Runtime;

namespace FubuTransportation.Testing.ScenarioSupport
{
    public class ThreeHandler : SimpleHandler<ThreeMessage>
    {
        public ThreeHandler(Envelope envelope) : base(envelope)
        {
        }
    }
}