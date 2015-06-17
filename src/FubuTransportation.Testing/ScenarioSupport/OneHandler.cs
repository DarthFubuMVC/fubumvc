using FubuTransportation.Runtime;

namespace FubuTransportation.Testing.ScenarioSupport
{
    public class OneHandler : SimpleHandler<OneMessage>
    {
        public OneHandler(Envelope envelope) : base(envelope)
        {
        }
    }
}