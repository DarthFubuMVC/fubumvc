using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.InMemory;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests.ServiceBus.InMemory
{
    [TestFixture]
    public class InMemoryReplyQueueIntegratedTester
    {
        private ChannelGraph graph;
        private ChannelNode theReplyNode;
        private FubuRuntime theRuntime;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            FubuTransport.AllQueuesInMemory = true;

            theRuntime = FubuRuntime.BasicBus();
            graph = theRuntime.Get<ChannelGraph>();

            var uri = graph.ReplyChannelFor(InMemoryChannel.Protocol);
            theReplyNode = graph.Single(x => x.Channel.Address == uri);
        }

        [TestFixtureTearDown]
        public void Teardown()
        {
            theRuntime.Dispose();
        }

        [Test]
        public void is_a_reply_node()
        {
            theReplyNode.ShouldNotBeNull();
        }

        [Test]
        public void should_be_incoming()
        {
            theReplyNode.Incoming.ShouldBeTrue();
        }

        [Test]
        public void key_is_derivative_from_the_transport()
        {
            theReplyNode.Key.ShouldBe("memory:replies");
        }

        [Test]
        public void should_have_a_channel()
        {
            theReplyNode.Channel.ShouldBeOfType<InMemoryChannel>();
        }
    }
}