using FubuCore;
using FubuMVC.Core.ServiceBus;

namespace FubuMVC.Tests.ServiceBus.ScenarioSupport
{
    public class MatchingMessageStep<T> : IScenarioStep where T : Message, new()
    {
        private readonly IOriginatingMessage _message;
        private readonly NodeConfiguration _receiver;

        public MatchingMessageStep(IOriginatingMessage message, NodeConfiguration receiver)
        {
            _message = message;
            _receiver = receiver;
        }

        public void PreviewAct(IScenarioWriter writer)
        {

        }

        public void PreviewAssert(IScenarioWriter writer)
        {
            if (_message.Description.IsEmpty())
            {
                writer.WriteLine("Expecting message of type {0} to be received by node {1} as a result of message of type {2} being handled", typeof(T).Name, _receiver.Name, _message.Message.GetType().Name);
            }
            else
            {
                writer.WriteLine("Expecting message of type {0} to be received by node {1} as a result of message of type {2} ({3}) being handled", typeof(T).Name, _receiver.Name, _message.Message.GetType().Name, _message.Description);
            }
        }

        public void Act(IScenarioWriter writer)
        {
            // no-op
        }

        public void Assert(IScenarioWriter writer)
        {
            if (!_receiver.Received(new T {Id = _message.Message.Id}))
            {
                writer.Failure("Message {0} was not received by {1}", typeof(T).Name, _receiver.Name);
            }
        }

        public bool MatchesSentMessage(Message processed)
        {
            return processed is T && processed.Id == _message.Message.Id;
        }
    }
}