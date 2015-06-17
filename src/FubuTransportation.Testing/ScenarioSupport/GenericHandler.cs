using FubuTransportation.Runtime;

namespace FubuTransportation.Testing.ScenarioSupport
{
    public class GenericHandler : SimpleHandler<Message>
    {
        public GenericHandler(Envelope envelope) : base(envelope)
        {
        }
    }
}