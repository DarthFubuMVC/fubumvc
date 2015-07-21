using System.Threading.Tasks;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.Services.Messaging.Tracking;

namespace FubuMVC.Tests.ServiceBus.ScenarioSupport
{
    public class SendAndAwaitStep<TRequest> : IScenarioStep where TRequest : Message, new()
    {
        private readonly NodeConfiguration _sender;
        private readonly TRequest _request;
        private Task _completion;

        public SendAndAwaitStep(NodeConfiguration sender)
        {
            _sender = sender;

            _request = new TRequest();
        }

        public void PreviewAct(IScenarioWriter writer)
        {
            writer.WriteLine("Node {0} sends and awaits an ack for request '{1}'", _sender.Name, typeof(TRequest).Name);
        }

        public void PreviewAssert(IScenarioWriter writer)
        {
            writer.WriteLine("Expecting an acknowledgement for the message");
        }

        public void Act(IScenarioWriter writer)
        {
            MessageHistory.Record(MessageTrack.ForSent(_request));
            _completion = _sender.ServiceBus.SendAndWait(_request);
        }

        public void Assert(IScenarioWriter writer)
        {
            if (!_completion.Wait(2000))
            {
                writer.Failure("Did not get an acknowledgement in time");
            }
        }

        public bool MatchesSentMessage(Message processed)
        {
            if (processed.GetType() == _request.GetType() && processed.Id == _request.Id) return true;

            return false;
        }
    }
}