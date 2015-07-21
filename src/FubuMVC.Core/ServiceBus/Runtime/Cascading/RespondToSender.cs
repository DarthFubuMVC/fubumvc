namespace FubuMVC.Core.ServiceBus.Runtime.Cascading
{
    public class RespondToSender : ISendMyself
    {
        private readonly object _outgoing;

        public RespondToSender(object outgoing)
        {
            _outgoing = outgoing;
        }

        public Envelope CreateEnvelope(Envelope original)
        {
            var response = original.ForResponse(_outgoing);
            response.Destination = original.ReplyUri;

            return response;
        }

        public object Outgoing
        {
            get { return _outgoing; }
        }
    }
}