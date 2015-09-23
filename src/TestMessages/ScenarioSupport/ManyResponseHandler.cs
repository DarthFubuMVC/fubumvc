using System.Collections.Generic;
using FubuMVC.Core.ServiceBus.Runtime;

namespace TestMessages.ScenarioSupport
{
    public class ManyResponseHandler<T, TR1, TR2, TR3> where T : Message, new()
                                                       where TR1 : Message, new()
                                                       where TR2 : Message, new()
                                                       where TR3 : Message, new()
    {
        private readonly Envelope _envelope;

        public ManyResponseHandler(Envelope envelope)
        {
            _envelope = envelope;
        }

        public IEnumerable<object> Handle(T message)
        {
            TestMessageRecorder.Processed(GetType().Name, message, _envelope.ReplyUri);

            yield return new TR1 {Id = message.Id};
            yield return new TR2 {Id = message.Id};
            yield return new TR3 {Id = message.Id};
        }
    }
}