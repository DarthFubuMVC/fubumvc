using FubuMVC.Tests.ServiceBus.ScenarioSupport;

namespace FubuMVC.Tests.ServiceBus.Scenarios
{
    public class Request_a_reply_for_a_single_message : Scenario
    {
        public Request_a_reply_for_a_single_message()
        {
            Website1.Registry.Channel(x => x.Service1)
                    .AcceptsMessage<OneMessage>();

            Service1.Handles<OneMessage>()
                .Raises<TwoMessage>();

            Website1.Requests<OneMessage>("original request")
                .ExpectReply<TwoMessage>().From(Service1);
        }


    }

    public class SendAndAwait_for_a_single_message : Scenario
    {
        public SendAndAwait_for_a_single_message()
        {
            Website1.Registry.Channel(x => x.Service1)
                    .AcceptsMessage<OneMessage>();

            Service1.Handles<OneMessage>();

            Website1.SendAndAwait<OneMessage>();
        }
    }
}