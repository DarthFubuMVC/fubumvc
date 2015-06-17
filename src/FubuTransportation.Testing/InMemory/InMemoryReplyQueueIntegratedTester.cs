using FubuMVC.Core;
using FubuTransportation.Configuration;
using FubuTransportation.InMemory;
using NUnit.Framework;
using FubuMVC.StructureMap;
using StructureMap;
using System.Linq;
using FubuTestingSupport;

namespace FubuTransportation.Testing.InMemory
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

            theRuntime = FubuTransport.For(x => { }).StructureMap(new Container()).Bootstrap();
            graph = theRuntime.Factory.Get<ChannelGraph>();

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
            theReplyNode.Key.ShouldEqual("memory:replies");
        }

        [Test]
        public void should_have_a_channel()
        {
            theReplyNode.Channel.ShouldBeOfType<InMemoryChannel>();
        }
    }
}