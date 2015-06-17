using FubuTransportation.Runtime;

namespace FubuTransportation.Testing.ScenarioSupport
{
    public class FourHandler : SimpleHandler<FourMessage>
    {
        public FourHandler(Envelope envelope) : base(envelope)
        {
        }
    }
}