using System;
using System.Linq;
using FubuMVC.Core.ServiceBus;
using Serenity;
using Serenity.ServiceBus;
using ServiceBusSerenitySamples.Setup;
using ServiceBusSerenitySamples.SystemUnderTest;
using ServiceBusSerenitySamples.SystemUnderTest.Subscriptions;
using StoryTeller;

namespace ServiceBusSerenitySamples.Fixtures
{
    public class MultipleEndpointsFixture : ExternalNodeFixture
    {
        private TestResponse _responseFromExternalService;

        public MultipleEndpointsFixture()
        {
            Title = "Multiple Endpoints";
        }

        [FormatAs("Add another service that communicates with the system under test")]
        public void SetupAnotherService()
        {
            AddOtherService();
        }

        [FormatAs("Send SomeCommand from node {name}")]
        public void SendCommandFromNode(string name)
        {
            var node = AddTestNode<ClientRegistry>(name);
            node.Send(new SomeCommand());
        }

        [FormatAs("Send message from node {name}")]
        public void SendMessageFromNode(string name)
        {
            var node = AddTestNode<ClientRegistry>(name);
            node.Send(new TestMessage());
        }

        [FormatAs("Send message from the system under test to the external service")]
        public void SendMessageToOtherService()
        {
            Retrieve<IServiceBus>().Send(new MessageForExternalService());
        }

        [FormatAs("Await a response to a request sent to the external service")]
        public void SendRequestToOtherService()
        {
            var node = AddOtherService();
            node.ClearReceivedMessages();

            var task = Retrieve<IServiceBus>().Request<TestResponse>(new MessageForExternalService());
            ShortWait(() => node.ReceivedMessages<MessageForExternalService>().Any());

            node.RespondToRequestWithMessage<MessageForExternalService>(new TestResponse());

            if (task.Wait(TimeSpan.FromSeconds(1)))
            {
                _responseFromExternalService = task.Result;
            }
        }

        [FormatAs("Node {name} received a response")]
        public bool NodeReceivedMessage(string name)
        {
            var node = AddTestNode<ClientRegistry>(name);
            return ShortWait(() => node.ReceivedMessage<TestResponse>());
        }

        [FormatAs("External service received the event")]
        public bool OtherServiceReceivedEvent()
        {
            return OtherServiceReceivedMessage<PublishedEvent>();
        }

        [FormatAs("External service received the message")]
        public bool OtherServiceReceivedMessage()
        {
            return OtherServiceReceivedMessage<MessageForExternalService>();
        }

        private bool OtherServiceReceivedMessage<T>()
        {
            var node = AddOtherService();
            return ShortWait(() => node.ReceivedMessage<T>());
        }

        [FormatAs("The system under test should receive the message")]
        public bool SystemReceivedMessage()
        {
            var messages = Context.Service<SystemUnderTest.MessageRecorder>().Messages;
            return ShortWait(() => messages.Any(x => x is TestMessage));
        }

        [FormatAs("The system under test should receive the response to the request")]
        public bool SystemReceivedRequestedResponse()
        {
            return _responseFromExternalService != null;
        }

        private ExternalNode AddOtherService()
        {
            return AddTestNode<AnotherServiceRegistry>("AnotherService");
        }

        private bool ShortWait(Func<bool> condition)
        {
            return Wait.Until(condition, 200, 2000);
        }
    }
}