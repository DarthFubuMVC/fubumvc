using System;
using System.Linq;
using FubuCore.Logging;
using FubuCore.Reflection;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Logging;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Headers;
using FubuMVC.Core.ServiceBus.Runtime.Routing;
using FubuMVC.Core.ServiceBus.Runtime.Serializers;
using FubuMVC.Core.ServiceBus.Scheduling;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using TestMessages;
using Is = Rhino.Mocks.Constraints.Is;

namespace FubuMVC.Tests.ServiceBus.Configuration
{
    [TestFixture]
    public class ChannelNodeTester
    {
        [Test]
        public void no_publishing_rules_is_always_false()
        {
            var node = new ChannelNode();
            node.Publishes(typeof(NewUser)).ShouldBeFalse();
        }

        [Test]
        public void publishes_is_true_if_any_rule_passes()
        {
            var node = new ChannelNode();
            for (int i = 0; i < 5; i++)
            {
                node.Rules.Add(MockRepository.GenerateMock<IRoutingRule>());
            }

            node.Rules[2].Stub(x => x.Matches(typeof (NewUser))).Return(true);

            node.Publishes(typeof(NewUser)).ShouldBeTrue();
        }

        [Test]
        public void publishes_is_false_if_no_rules_pass()
        {
            var node = new ChannelNode();
            for (int i = 0; i < 5; i++)
            {
                node.Rules.Add(MockRepository.GenerateMock<IRoutingRule>());
            }


            node.Publishes(typeof(NewUser)).ShouldBeFalse();
        }

        [Test]
        public void setting_address_has_to_be_a_Uri()
        {
            var node = new ChannelNode();
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() => {
                node.SettingAddress = ReflectionHelper.GetAccessor<FakeThing>(x => x.Name);
            });
        }

        [Test]
        public void setting_default_content_type_will_clear_the_serializer()
        {
            var node = new ChannelNode();
            node.DefaultSerializer = new BinarySerializer();

            node.DefaultContentType = "application/xml";

            node.DefaultContentType.ShouldBe("application/xml");
            node.DefaultSerializer.ShouldBeNull();
        }

        [Test]
        public void setting_the_default_serializer_will_clear_the_default_content_type()
        {
            var node = new ChannelNode
            {
                DefaultContentType = "application/xml"
            };

            node.DefaultSerializer = new BinarySerializer();

            node.DefaultSerializer.ShouldBeOfType<BinarySerializer>();
            node.DefaultContentType.ShouldBeNull();
        }

        public void start_receiving()
        {
            if (DateTime.Today > new DateTime(2013, 11, 21))
            {
                Assert.Fail("Jeremy needs to fix the structure so that this is possible");
            }

            

//            var invoker = MockRepository.GenerateMock<IHandlerPipeline>();
//
//            var node = new ChannelNode
//            {
//                Incoming = true,
//                Channel = MockRepository.GenerateMock<IChannel>(),
//                Scheduler = new FakeScheduler()
//            };
//
//            var graph = new ChannelGraph();
//
//            var startingVisitor = new StartingChannelNodeVisitor(new Receiver(invoker, graph, node));
//            startingVisitor.Visit(node);
//            
//
//
//            node.Channel.AssertWasCalled(x => x.Receive(new Receiver(invoker, graph, node)));
        }

        [Test]
        public void ReceiveFailed_error_handling()
        {
            var node = new ChannelNode
            {
                Key = "TestKey",
                Channel = new FakeChannel { StopAfter = 2 },
                Scheduler = new FakeScheduler()
            };

            var logger = new RecordingLogger();
            node.StartReceiving(new RecordingReceiver(), logger);

            logger.ErrorMessages.ShouldHaveCount(1);
            logger.InfoMessages.ShouldHaveCount(1);
            var message = logger.InfoMessages.Cast<ReceiveFailed>().Single();
            message.ChannelKey.ShouldBe(node.Key);
            message.Exception.ShouldNotBeNull();
        }

        [Test]
        public void continuous_receive_errors()
        {
            var logger = new RecordingLogger();
            var receiver = new RecordingReceiver();
            var channel = MockRepository.GenerateMock<IChannel>();
            channel.Expect(x => x.Receive(receiver))
                .Throw(new Exception("I failed"));

            var node = new ChannelNode
            {
                Channel = channel,
                Scheduler = new FakeScheduler()
            };

            Exception<ReceiveFailureException>.ShouldBeThrownBy(() =>
            {
                node.StartReceiving(receiver, logger);
            });

        }

        [Test]
        public void doesnt_throw_if_receive_only_fails_intermittently()
        {
            var channel = new FakeChannel { StopAfter = 20 };
            var node = new ChannelNode
            {
                Channel = channel,
                Scheduler = new FakeScheduler()
            };

            var logger = new RecordingLogger();
            var receiver = new RecordingReceiver();
            node.StartReceiving(receiver, logger);

            channel.HitCount.ShouldBe(20);
        }

        public class FakeChannel : IChannel
        {
            public int HitCount { get; private set; }
            public int StopAfter { get; set; }

            public ReceivingState Receive(IReceiver receiver)
            {
                if (++HitCount >= StopAfter)
                    return ReceivingState.StopReceiving;

                // Throw every other time
                if (HitCount % 2 == 1)
                    throw new Exception("I failed");

                return ReceivingState.CanContinueReceiving;
            }

            public Uri Address { get; private set; }
            public void Send(byte[] data, IHeaders headers)
            {
            }

            public void Dispose()
            {
            }
        }
    }

    public class FakeScheduler : IScheduler
    {
        public void Dispose()
        {
            
        }

        public void Start(Action action)
        {
            action();
        }
    }

    [TestFixture]
    public class when_sending_an_envelope
    {
        private Envelope theEnvelope;
        private RecordingChannel theChannel;
        private ChannelNode theNode;
        private IEnvelopeSerializer theSerializer;

        [SetUp]
        public void SetUp()
        {
            theEnvelope = new Envelope()
            {
                Data = new byte[]{1,2,3,4},
                
            };

            theSerializer = MockRepository.GenerateMock<IEnvelopeSerializer>();

            theEnvelope.Headers["A"] = "1";
            theEnvelope.Headers["B"] = "2";
            theEnvelope.Headers["C"] = "3";
            theEnvelope.CorrelationId = Guid.NewGuid().ToString();

            theChannel = new RecordingChannel();

            theNode = new ChannelNode
            {
                Channel = theChannel,
                Key = "Foo",
                Uri = "foo://bar".ToUri()
            };

            theNode.Modifiers.Add(new HeaderSetter("D", "4"));
            theNode.Modifiers.Add(new HeaderSetter("E", "5"));

            theNode.Send(theEnvelope, theSerializer);
        }

        public class HeaderSetter : IEnvelopeModifier
        {
            private readonly string _key;
            private readonly string _value;

            public HeaderSetter(string key, string value)
            {
                _key = key;
                _value = value;
            }

            public void Modify(Envelope envelope)
            {
                envelope.Headers[_key] = _value;
            }
        }

        [Test]
        public void should_serialize_the_envelope()
        {
            theSerializer.AssertWasCalled(x => x.Serialize(null, theNode), x => {
                x.Constraints(Is.Matching<Envelope>(o => {
                    o.CorrelationId.ShouldBe(theEnvelope.CorrelationId);
                    o.ShouldNotBeTheSameAs(theEnvelope);


                    return true;
                }), Is.Same(theNode));
            });
        }

        [Test]
        public void should_have_applied_the_channel_specific_modifiers()
        {
            var sentHeaders = theChannel.Sent.Single().Headers;
            sentHeaders["D"].ShouldBe("4");
            sentHeaders["E"].ShouldBe("5");
        }

 
        [Test]
        public void should_have_sent_a_copy_of_the_headers()
        {
            var sentHeaders = theChannel.Sent.Single().Headers;
            sentHeaders.ShouldNotBeTheSameAs(theEnvelope.Headers);

            sentHeaders["A"].ShouldBe("1");
            sentHeaders["B"].ShouldBe("2");
            sentHeaders["C"].ShouldBe("3");
        }

        [Test]
        public void sends_the_channel_key()
        {
            var sentHeaders = theChannel.Sent.Single().Headers;
            sentHeaders[Envelope.ChannelKey].ShouldBe(theNode.Key);
        }

        [Test]
        public void sends_the_destination_as_a_header()
        {
            var sentHeaders = theChannel.Sent.Single().Headers;
            sentHeaders[Envelope.DestinationKey].ToUri().ShouldBe(theNode.Uri);
        }
    }

    public class FakeThing
    {
        public string Name { get; set; }
    }
}