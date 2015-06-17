using FubuTransportation.Runtime;

namespace FubuTransportation.Testing.ScenarioSupport
{
    public class DifferentOneHandler : SimpleHandler<OneMessage>
    {
        public DifferentOneHandler(Envelope envelope) : base(envelope)
        {
        }
    }
}