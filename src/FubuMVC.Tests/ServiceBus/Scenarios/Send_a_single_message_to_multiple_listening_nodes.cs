using FubuMVC.Tests.ServiceBus.ScenarioSupport;

namespace FubuMVC.Tests.ServiceBus.Scenarios
{
    public class Send_a_single_message_to_multiple_listening_nodes : Scenario
    {
        public Send_a_single_message_to_multiple_listening_nodes()
        {
            Website1.Registry.Channel(x => x.Service1)
                    .AcceptsMessage<OneMessage>();

            Website1.Registry.Channel(x => x.Service3)
                    .AcceptsMessage<OneMessage>();


            Service1.Handles<OneMessage>();
            Service2.Handles<OneMessage>();
            Service3.Handles<OneMessage>();
            Service4.Handles<OneMessage>();

            Website1.Sends<OneMessage>("first message")
                .ShouldBeReceivedBy(Service1)
                .ShouldBeReceivedBy(Service3);
        }
    }
}