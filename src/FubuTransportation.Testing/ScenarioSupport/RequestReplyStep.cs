using System.Threading.Tasks;
using Bottles.Services.Messaging.Tracking;

namespace FubuTransportation.Testing.ScenarioSupport
{
    public class RequestReplyStep<TRequest, TReply> : IScenarioStep where TRequest : Message, new() where TReply : Message
    {
        private readonly string _description;
        private readonly NodeConfiguration _sender;
        private readonly NodeConfiguration _receiver;
        private Task<TReply> _completion;
        private readonly TRequest _request;

        public RequestReplyStep(string description, NodeConfiguration sender, NodeConfiguration receiver)
        {
            _description = description;
            _sender = sender;
            _receiver = receiver;

            _request = new TRequest();
        }

        public void PreviewAct(IScenarioWriter writer)
        {
            writer.WriteLine("Node {0} sends request '{1}' ({2}), expecting a matching response {3}", _sender.Name, _description, typeof(TRequest).Name, typeof(TReply).Name);
        }

        public void PreviewAssert(IScenarioWriter writer)
        {
            writer.WriteLine("Expecting a reply of type {0} from node {1}", typeof(TReply).Name, _receiver.Name);
        }

        public void Act(IScenarioWriter writer)
        {
            MessageHistory.Record(MessageTrack.ForSent(_request));
            _completion = _sender.ServiceBus.Request<TReply>(_request);
        }

        public void Assert(IScenarioWriter writer)
        {
            if (_completion.Wait(2000))
            {
                var response = _completion.Result;

                if (response.Id != _request.Id)
                {
                    writer.Failure("ResponseIdKey does not match the request");
                }
            }
            else
            {
                writer.Failure("Did not get any response!");
            }

        }

        public bool MatchesSentMessage(Message processed)
        {
            if (processed.GetType() == _request.GetType() && processed.Id == _request.Id) return true;

            return processed is TReply && processed.Id == _request.Id;
        }
    }
}