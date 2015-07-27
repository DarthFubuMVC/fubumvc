using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Serializers;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Runtime.Serializers
{
    [TestFixture]
    public class EnvelopeSerializerIntegratedTester
    {
        private IMessageSerializer[] messageSerializers;
        private EnvelopeSerializer theSerializer;
        private Address theAddress;
        private ChannelGraph theGraph;

        [SetUp]
        public void SetUp()
        {
            messageSerializers = new IMessageSerializer[]
            {new BinarySerializer(), new BasicJsonMessageSerializer(), new XmlMessageSerializer()};
            theGraph = new ChannelGraph();
            theSerializer = new EnvelopeSerializer(theGraph, messageSerializers);

            theAddress = new Address {City = "Jasper", State = "Missouri"};
        }

        private void assertRoundTrips(int index)
        {
            var contentType = messageSerializers[index].ContentType;
            var envelope = new Envelope()
            {
                Message = theAddress,
                ContentType = contentType
            };

            theSerializer.Serialize(envelope, new ChannelNode());

            envelope.Message = theSerializer.Deserialize(envelope);

            envelope.Message.ShouldNotBeTheSameAs(theAddress);
            envelope.Message.ShouldBe(theAddress);
        }

        [Test]
        public void can_round_trip_with_each_serializer_type()
        {
            assertRoundTrips(0);
            assertRoundTrips(1);
            assertRoundTrips(2);
        }

        [Test]
        public void happily_chooses_the_default_content_type_for_the_graph_if_none_is_on_the_envelope()
        {
            var envelope = new Envelope()
            {
                Message = theAddress,
                ContentType = null
            };

            theSerializer.Serialize(envelope, new ChannelNode());

            envelope.ContentType.ShouldBe(theGraph.DefaultContentType);

            envelope.Message = theSerializer.Deserialize(envelope);

            envelope.Message.ShouldNotBeTheSameAs(theAddress);
            envelope.Message.ShouldBe(theAddress);

        }
    }

}