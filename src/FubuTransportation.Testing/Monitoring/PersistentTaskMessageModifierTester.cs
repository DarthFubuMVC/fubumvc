using System;
using FubuTestingSupport;
using FubuTransportation.Configuration;
using FubuTransportation.Monitoring;
using NUnit.Framework;

namespace FubuTransportation.Testing.Monitoring
{
    [TestFixture]
    public class PersistentTaskMessageModifierTester
    {
        private readonly ChannelGraph theChannel = new ChannelGraph
        {
            NodeId = "TheNodeId",

        };

        [Test]
        public void adds_the_machine_name_and_channel_node_id_to_the_message()
        {
            var modifier = new PersistentTaskMessageModifier(theChannel);
            var message = new TaskActivationFailure("foo://1".ToUri());
            modifier.Modify(message);

            message.NodeId.ShouldEqual(theChannel.NodeId);
            message.Machine.ShouldEqual(Environment.MachineName);
        }
    }
}