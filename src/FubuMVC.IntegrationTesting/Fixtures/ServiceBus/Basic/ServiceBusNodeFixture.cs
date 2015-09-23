using System.Collections.Generic;
using System.Linq;
using FubuCore.Reflection;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Runtime.Routing;
using StoryTeller;
using StoryTeller.Grammars.Tables;

namespace FubuMVC.IntegrationTesting.Fixtures.ServiceBus.Basic
{
    [Hidden]
    public class ServiceBusNodeFixture : Fixture
    {
        private NodeRegistry _registry;

        public ServiceBusNodeFixture()
        {
            Title = "Active Service Bus Node";

            AddSelectionValues("Channels", ServiceBusNodes.Channels.Select(x => x.Name).ToArray());
            AddSelectionValues("Messages", ServiceBusNodes.MessageTypes.Select(x => x.Name).ToArray());
        }

        public override void TearDown()
        {
            Context.State.Retrieve<ServiceBusNodes>().Start(_registry);
        }

        [FormatAs("Node {name} listens to {channels}")]
        public void NodeName([SelectionList("Channels")]string name, string[] channels)
        {
            _registry = Context.State.Retrieve<ServiceBusNodes>().CreateNew(name);
            channels.Each(channelName =>
            {
                var prop = ServiceBusNodes.Channels.First(x => x.Name == name);
                _registry.AlterSettings<ChannelGraph>(graph =>
                {
                    var channel = graph.ChannelFor(new SingleProperty(prop));
                    channel.Incoming = true;
                });
            });
        }



        [ExposeAsTable("Publishes")]
        public void Publishes([SelectionList("Channels")] string Channel, [SelectionList("Messages")] string Message)
        {
            _registry.AlterSettings<ChannelGraph>(graph =>
            {
                var prop = ServiceBusNodes.Channels.First(x => x.Name == Channel);
                var channel = graph.ChannelFor(new SingleProperty(prop));
                var messageType = ServiceBusNodes.FindMessageType(Message);
                channel.Rules.Add(new LambdaRoutingRule(type => type == messageType));
            });
        }

        [ExposeAsTable("Messages with Replies")]
        public void Replies([SelectionList("Messages")] string Message, [SelectionList("Messages")] string Reply)
        {
            _registry.AddReply(ServiceBusNodes.FindMessageType(Message), ServiceBusNodes.FindMessageType(Reply));
        }

    }
}