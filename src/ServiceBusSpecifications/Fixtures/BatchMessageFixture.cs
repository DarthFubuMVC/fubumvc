using FubuMVC.Core.ServiceBus;
using Serenity.ServiceBus;
using ServiceNode;
using StoryTeller;

namespace ServiceBusSpecifications.Fixtures
{
    public class BatchMessageFixture : Fixture
    {
        public BatchMessageFixture()
        {
            Title = "Batch Message Mechanics";
        }

        public IGrammar SendBatch()
        {
            return Embed<DefineBatchMessageFixture>("Send a batch of messages");
        }

        public IGrammar TheRecordedMessagesAre()
        {
            return
                VerifyStringList(() => TextFileWriter.Read())
                    .Titled("The recorded message log from the service should be");
        }
    }

    [Hidden]
    public class DefineBatchMessageFixture : FubuTransportActFixture
    {
        private MessageBatch _batch;

        protected override void setup()
        {
            _batch = new MessageBatch();
        }

        [FormatAs("Description is {description}")]
        public void DescriptionIs(string description)
        {
            _batch.Description = description;
        }

        [FormatAs("Add color {color}")]
        public void SendColor(string color)
        {
            _batch.Add(new ColorMessage {Color = color});
        }

        [FormatAs("Add direction {direction}")]
        public void SendDirection(string direction)
        {
            _batch.Add(new DirectionMessage {Direction = direction});
        }

        [FormatAs("Add state {state}")]
        public void SendState(string state)
        {
            _batch.Add(new StateMessage {State = state});
        }

        [FormatAs("Add team {team}")]
        public void SendTeam(string team)
        {
            _batch.Add(new TeamMessage {Team = team});
        }

        protected override void teardown()
        {
            Retrieve<IServiceBus>().Send(_batch);
        }
    }
}