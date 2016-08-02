using FubuMVC.Core.ServiceBus.Runtime.Invocation;
using FubuMVC.Core.ServiceBus.Runtime.Invocation.Batching;

namespace ServiceNode
{
    public class DirectionMessage
    {
        public string Direction { get; set; }

        public override string ToString()
        {
            return $"Direction: {Direction}";
        }
    }

    public class ColorMessage
    {
        public string Color { get; set; }

        public override string ToString()
        {
            return $"Color: {Color}";
        }
    }

    public class StateMessage
    {
        public string State { get; set; }

        public override string ToString()
        {
            return $"State: {State}";
        }
    }

    public class TeamMessage
    {
        public string Team { get; set; }

        public override string ToString()
        {
            return $"Team: {Team}";
        }
    }

    public abstract class RecordingHandler<T>
    {
        public void Handle(T message)
        {
            TextFileWriter.Write(message.ToString());
        }
    }

    public class DirectionHandler : RecordingHandler<DirectionMessage>{}
    public class ColorHandler : RecordingHandler<ColorMessage>{}
    public class StateHandler : RecordingHandler<StateMessage>{}
    public class TeamHandler : RecordingHandler<TeamMessage>{}


    public class MessageBatch : BatchMessage
    {
        public string Description { get; set; }
    }

    public class MessageBatchConsumer : BatchConsumer<MessageBatch>
    {
        public MessageBatchConsumer(IMessageExecutor executor) : base(executor)
        {
        }

        public override void BatchStart(MessageBatch batch)
        {
            TextFileWriter.Write("Starting: " + batch.Description);
        }

        public override void BatchFinish(MessageBatch batch)
        {
            TextFileWriter.Write("Finishing: " + batch.Description);
        }
    }

}