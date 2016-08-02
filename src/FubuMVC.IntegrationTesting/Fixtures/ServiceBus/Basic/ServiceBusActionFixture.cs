using System.Linq;
using StoryTeller;
using StoryTeller.Grammars.Tables;
using MessageHistory = FubuMVC.Core.Services.Messaging.Tracking.MessageHistory;

namespace FubuMVC.IntegrationTesting.Fixtures.ServiceBus.Basic
{
    [Hidden]
    public class ServiceBusActionFixture : Fixture
    {
        public ServiceBusActionFixture()
        {
            AddSelectionValues("Channels", ServiceBusNodes.Channels.Select(x => x.Name).ToArray());
            AddSelectionValues("Messages", ServiceBusNodes.MessageTypes.Select(x => x.Name).ToArray());
        }


        [ExposeAsTable("Send messages")]
        public void Send(string Key, [SelectionList("Channels")]string Node, [SelectionList("Messages")]string Message)
        {
            MessageHistory.WaitForWorkToFinish(() =>
            {
                Context.State.Retrieve<ServiceBusNodes>().Send(Key, Node, Message);
            });

            MessageHistory.AssertFinished();
        }


    }
}