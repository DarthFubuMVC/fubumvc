using System;
using System.Linq;
using System.Linq.Expressions;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Runtime.Routing;
using FubuMVC.Core.ServiceBus.Runtime.Serializers;
using FubuMVC.Core.ServiceBus.Scheduling;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus
{
    [TestFixture]
    public class IntegratedFubuTransportRegistryTester
    {
        [SetUp]
        public void SetUp()
        {
            theRegistry = new BusRegistry();
            _behaviors = new Lazy<BehaviorGraph>(() =>
            {
                return BehaviorGraph.BuildFrom(theRegistry);
            });

            _channels = new Lazy<ChannelGraph>(() => _behaviors.Value.Settings.Get<ChannelGraph>());
        }

        private BusRegistry theRegistry;
        private Lazy<ChannelGraph> _channels;
        private Lazy<BehaviorGraph> _behaviors;


        public ChannelGraph theChannels
        {
            get { return _channels.Value; }
        }

        public ChannelNode channelFor(Expression<Func<BusSettings, Uri>> expression)
        {
            return theChannels.ChannelFor(expression);
        }

        [Test]
        public void set_channel_to_listening()
        {
            theRegistry.Channel(x => x.Upstream).ReadIncoming();

            channelFor(x => x.Upstream).Incoming.ShouldBeTrue();
            channelFor(x => x.Downstream).Incoming.ShouldBeFalse();
        }

        [Test]
        public void set_channel_to_listening_override_thread_count()
        {
            theRegistry.Channel(x => x.Upstream).ReadIncoming(new ThreadScheduler(5));

            channelFor(x => x.Upstream).Incoming.ShouldBeTrue();
            channelFor(x => x.Upstream).Scheduler.As<ThreadScheduler>().ThreadCount.ShouldBe(5);

            channelFor(x => x.Downstream).Incoming.ShouldBeFalse();
        }

        [Test]
        public void set_the_default_content_type_by_serializer_type()
        {
            theRegistry.ServiceBus.DefaultSerializer<BinarySerializer>();

            theChannels.DefaultContentType.ShouldBe(new BinarySerializer().ContentType);
        }

        [Test]
        public void set_the_default_content_type_by_string()
        {
            theRegistry.ServiceBus.DefaultContentType("application/json");
            theChannels.DefaultContentType.ShouldBe("application/json");
        }

        [Test]
        public void set_the_default_content_type_for_a_channel_by_serializer()
        {
            theRegistry.Channel(x => x.Outbound).DefaultSerializer<BinarySerializer>();
            theRegistry.Channel(x => x.Downstream).DefaultSerializer<XmlMessageSerializer>();
            theRegistry.Channel(x => x.Upstream).DefaultSerializer<BasicJsonMessageSerializer>();

            channelFor(x => x.Outbound).DefaultSerializer.ShouldBeOfType<BinarySerializer>();
            channelFor(x => x.Downstream).DefaultSerializer.ShouldBeOfType<XmlMessageSerializer>();
            channelFor(x => x.Upstream).DefaultSerializer.ShouldBeOfType<BasicJsonMessageSerializer>();
        }

        [Test]
        public void set_the_default_content_type_for_a_channel_by_string()
        {
            theRegistry.Channel(x => x.Outbound).DefaultContentType("application/json");
            channelFor(x => x.Outbound).DefaultContentType.ShouldBe("application/json");
        }

        [Test]
        public void add_namespace_publishing_rule()
        {
            theRegistry.Channel(x => x.Outbound).AcceptsMessagesInNamespaceContainingType<BusSettings>();

            channelFor(x => x.Outbound).Rules.Single()
                .ShouldBe(NamespaceRule.For<BusSettings>());

            channelFor(x => x.Upstream).Rules.Any().ShouldBeFalse();
        }

        [Test]
        public void add_namespace_publishing_rule_2()
        {
            theRegistry.Channel(x => x.Outbound).AcceptsMessagesInNamespace(typeof (BusSettings).Namespace);

            channelFor(x => x.Outbound).Rules.Single()
                .ShouldBe(NamespaceRule.For<BusSettings>());

            channelFor(x => x.Upstream).Rules.Any().ShouldBeFalse();
        }

        [Test]
        public void add_assembly_publishing_rule()
        {
            theRegistry.Channel(x => x.Outbound).AcceptsMessagesInAssemblyContainingType<BusSettings>();

            channelFor(x => x.Outbound).Rules.Single()
                .ShouldBe(AssemblyRule.For<BusSettings>());

            channelFor(x => x.Upstream).Rules.Any().ShouldBeFalse();
        }

        [Test]
        public void add_assembly_publishing_rule_2()
        {
            theRegistry.Channel(x => x.Outbound).AcceptsMessagesInAssembly(typeof (BusSettings).Assembly.GetName().Name);

            channelFor(x => x.Outbound).Rules.Single()
                .ShouldBe(AssemblyRule.For<BusSettings>());

            channelFor(x => x.Upstream).Rules.Any().ShouldBeFalse();
        }

        [Test]
        public void add_single_type_rule()
        {
            theRegistry.Channel(x => x.Outbound).AcceptsMessage<BusSettings>();
            channelFor(x => x.Outbound).Rules.Single()
                .ShouldBeOfType<SingleTypeRoutingRule<BusSettings>>();

            channelFor(x => x.Upstream).Rules.Any().ShouldBeFalse();
        }

        [Test]
        public void add_single_type_rule_2()
        {
            theRegistry.Channel(x => x.Outbound).AcceptsMessage(typeof (BusSettings));
            channelFor(x => x.Outbound).Rules.Single()
                .ShouldBeOfType<SingleTypeRoutingRule<BusSettings>>();

            channelFor(x => x.Upstream).Rules.Any().ShouldBeFalse();
        }

        [Test]
        public void add_adhoc_rule()
        {
            theRegistry.Channel(x => x.Outbound).AcceptsMessages(type => type == typeof (BusSettings));

            var rule = channelFor(x => x.Outbound).Rules.Single();

            rule.ShouldBeOfType<LambdaRoutingRule>();
            rule.Matches(typeof (BusSettings)).ShouldBeTrue();
            rule.Matches(GetType()).ShouldBeFalse();
        }

        [Test]
        public void add_custom_rule()
        {
            theRegistry.Channel(x => x.Outbound).AcceptsMessagesMatchingRule<CustomRule>();

            channelFor(x => x.Outbound).Rules.Single()
                .ShouldBeOfType<CustomRule>();
        }
    }

    public class BusSettings
    {
        public BusSettings()
        {
            DownstreamCount = 2;
        }

        public Uri Outbound { get; set; }
        public Uri Downstream { get; set; }
        public Uri Upstream { get; set; }
        public int DownstreamCount { get; private set; }
    }

    public class CustomRule : IRoutingRule
    {
        public bool Matches(Type type)
        {
            throw new NotImplementedException();
        }

        public string Describe()
        {
            return "Custom!";
        }
    }

    public class BusRegistry : FubuTransportRegistry<BusSettings>
    {
    }
}