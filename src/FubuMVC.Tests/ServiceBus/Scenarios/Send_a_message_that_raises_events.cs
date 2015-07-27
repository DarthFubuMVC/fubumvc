using FubuMVC.Tests.ServiceBus.ScenarioSupport;

namespace FubuMVC.Tests.ServiceBus.Scenarios
{
    public class Send_a_message_that_raises_events : Scenario
    {
        public Send_a_message_that_raises_events()
        {
            Website1.Registry.Channel(x => x.Service1)
                    .AcceptsMessage<OneMessage>();

            Service1.Handles<OneMessage>()
                .Raises<TwoMessage>()
                .Raises<ThreeMessage>();

            Service1.Handles<TwoMessage>();
            Service1.Handles<ThreeMessage>();

            // TODO -- super hokey
            Service1.Registry.Channel(x => x.Service1)
                    .AcceptsMessage<TwoMessage>().AcceptsMessage<ThreeMessage>();

            // Assuming that if the events raised can be handled locally, they are
            // handled here. Corey/Ryan to review
            Website1.Sends<OneMessage>("original message")
                .ShouldBeReceivedBy(Service1)
                .MatchingMessageIsReceivedBy<TwoMessage>(Service1)
                .MatchingMessageIsReceivedBy<ThreeMessage>(Service1);
        }
    }
}