using FubuMVC.Tests.ServiceBus.ScenarioSupport;

namespace FubuTransportation.Testing.Scenarios
{
    public class Send_a_single_message_to_the_correct_node : Scenario
    {
        public Send_a_single_message_to_the_correct_node()
        {
            Website1.Registry.Channel(x => x.Service1)
                    .AcceptsMessage<OneMessage>();

            Service1.Handles<OneMessage>();

            Service2.Handles<OneMessage>();
            Service3.Handles<OneMessage>();

            Website1.Sends<OneMessage>("first message").ShouldBeReceivedBy(Service1);
        }
    }
}