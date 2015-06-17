using FubuTransportation.Runtime;

namespace FubuTransportation.Testing.ScenarioSupport
{
    public class AnotherOneHandler : SimpleHandler<OneMessage>
    {
        public AnotherOneHandler(Envelope envelope) : base(envelope)
        {
        }
    }
}