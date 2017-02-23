using System.Linq;
using FubuMVC.Core.Services.Messaging.Tracking;
using StoryTeller;
using StoryTeller.Grammars.Tables;

namespace Specifications.Fixtures.ServiceBus.Basic
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