using FubuMVC.Core.ServiceBus.Runtime;

namespace TestMessages.ScenarioSupport
{
    public class RequestResponseHandler<T> where T : Message
    {
        private readonly Envelope _envelope;

        public RequestResponseHandler(Envelope envelope)
        {
            _envelope = envelope;
        }

        public MirrorMessage<T> Handle(T message)
        {
            TestMessageRecorder.Processed(GetType().Name, message, _envelope.ReceivedAt);
            return new MirrorMessage<T> {Id = message.Id};
        }
    }

    public class RequestResponseHandler<TRequest, TResponse> where TRequest : Message where TResponse : Message, new()
    {
        public TResponse Handle(TRequest request)
        {
            return new TResponse
            {
                Id = request.Id,
                Key = request.Key
            };
        }
    }
}