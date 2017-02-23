using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Subscriptions;
using FubuMVC.Tests.TestSupport;
using Xunit;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Subscriptions
{
    
    public class when_handling_subscriptions_changed : InteractionContext<SubscriptionsHandler>
    {
        private Subscription[] theSubscriptions;
        private ChannelGraph theGraph;

        protected override void beforeEach()
        {
            theSubscriptions = new Subscription[]
            {
                ObjectMother.ExistingSubscription(),
                ObjectMother.ExistingSubscription(),
                ObjectMother.ExistingSubscription(),
                ObjectMother.ExistingSubscription(),
                ObjectMother.ExistingSubscription()
            };

            theGraph = new ChannelGraph {Name = "TheNode"};
            Services.Inject(theGraph);

            MockFor<ISubscriptionRepository>().Stub(x => x.LoadSubscriptions(SubscriptionRole.Publishes))
                .Return(theSubscriptions);

            ClassUnderTest.Handle(new SubscriptionsChanged());
        }

        [Fact]
        public void should_load_the_new_subscriptions_into_the_running_cache()
        {
            MockFor<ISubscriptionCache>()
                .AssertWasCalled(x => x.LoadSubscriptions(theSubscriptions));
        }
    }

    
    public class when_sending_a_subscription_to_a_peer : InteractionContext<SubscriptionsHandler>
    {
        private RecordingEnvelopeSender theSender;
        private TransportNode thePeer;

        protected override void beforeEach()
        {
            theSender = new RecordingEnvelopeSender();
            Services.Inject<IEnvelopeSender>(theSender);

            thePeer = new TransportNode
            {
                Addresses = new Uri[] {"memory://replies".ToUri()},
                ControlChannel = "memory://1".ToUri()
            };

            ClassUnderTest.SendSubscriptionChangedToPeer(thePeer);
        }

        [Fact]
        public void should_have_sent_a_message_to_the_peer_to_reload_subscriptions()
        {
            var envelope = theSender.Sent.Single();
            envelope.Message.ShouldBeOfType<SubscriptionsChanged>();
            envelope.Destination.ShouldBe(thePeer.ControlChannel);
        }
    }

    
    public class when_handling_the_subscriptions_changed_message : InteractionContext<SubscriptionsHandler>
    {
        private TransportNode[] thePeers;
        private SubscriptionRequested theMessage;

        protected override void beforeEach()
        {
            Services.PartialMockTheClassUnderTest();

            thePeers = new TransportNode[]
            {
                new TransportNode(),
                new TransportNode(),
                new TransportNode(),
            };

            ClassUnderTest.Expect(x => x.ReloadSubscriptions());
            thePeers.Each(peer => ClassUnderTest.Expect(o => o.SendSubscriptionChangedToPeer(peer)));

            MockFor<ISubscriptionRepository>().Stub(x => x.FindPeers())
                .Return(thePeers);

            theMessage = new SubscriptionRequested
            {
                Subscriptions = new Subscription[]
                {
                    ObjectMother.ExistingSubscription(),
                    ObjectMother.ExistingSubscription(),
                    ObjectMother.ExistingSubscription(),
                    ObjectMother.ExistingSubscription(),
                    ObjectMother.ExistingSubscription()
                }
            };

            ClassUnderTest.Handle(theMessage);
        }

        [Fact]
        public void should_persist_the_new_subscriptions()
        {
            MockFor<ISubscriptionRepository>()
                .AssertWasCalled(x => x.PersistPublishing(theMessage.Subscriptions));
        }

        [Fact]
        public void should_reload_subscriptions()
        {
            ClassUnderTest.AssertWasCalled(x => x.ReloadSubscriptions());
        }

        [Fact]
        public void should_message_all_of_its_peers_to_reload_their_subscriptions()
        {
            thePeers.Each(peer => {
                ClassUnderTest.AssertWasCalled(x => x.SendSubscriptionChangedToPeer(peer));
            });
        }
    }

    
    public class when_handling_subscriptions_removed : InteractionContext<SubscriptionsHandler>
    {
        private SubscriptionsRemoved theMessage;

        protected override void beforeEach()
        {
            Services.PartialMockTheClassUnderTest();

            ClassUnderTest.Expect(x => x.ReloadSubscriptions());
            ClassUnderTest.Expect(x => x.UpdatePeers());

            theMessage = new SubscriptionsRemoved
            {
                Receiver = new Uri("memory://receiver")
            };
            ClassUnderTest.Handle(theMessage);
        }

        [Fact]
        public void should_remove_subscriptions_from_repository()
        {
            MockFor<ISubscriptionRepository>()
                .AssertWasCalled(x => x.RemoveSubscriptionsForReceiver(theMessage.Receiver));
        }

        [Fact]
        public void should_reload_subscriptions()
        {
            ClassUnderTest.AssertWasCalled(x => x.ReloadSubscriptions());
        }

        [Fact]
        public void should_update_peers()
        {
            ClassUnderTest.AssertWasCalled(x => x.UpdatePeers());
        }
    }
}