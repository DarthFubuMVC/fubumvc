using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Subscriptions;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Subscriptions
{
    [TestFixture]
    public class SubscriptionRepositoryTester
    {
        private InMemorySubscriptionPersistence persistence;
        private SubscriptionRepository theRepository;
        private string TheNodeName = "TheNode";
        private ChannelGraph channelGraph;

        [SetUp]
        public void SetUp()
        {
            persistence = new InMemorySubscriptionPersistence();
            channelGraph = new ChannelGraph{Name = TheNodeName};
            channelGraph.AddReplyChannel("foo", "foo://replies".ToUri());
            channelGraph.AddReplyChannel("bar", "bar://replies".ToUri());

            theRepository = new SubscriptionRepository(channelGraph, persistence);
        }

        [Test]
        public void save_the_first_subscriptions()
        {
            var subscription = ObjectMother.NewSubscription();

            theRepository.PersistSubscriptions(subscription);

            var requirements = theRepository.LoadSubscriptions(SubscriptionRole.Subscribes);
            requirements
                .ShouldHaveTheSameElementsAs(subscription);

            requirements.Single().Id.ShouldNotBe(Guid.Empty);
        }

        [Test]
        public void save_a_new_subscription_that_does_not_match_existing()
        {
            var existing = ObjectMother.ExistingSubscription();
            existing.NodeName = TheNodeName;

            persistence.Persist(existing);

            var subscription = ObjectMother.NewSubscription();
            subscription.NodeName = TheNodeName;

            theRepository.PersistSubscriptions(subscription);
            var requirements = theRepository.LoadSubscriptions(SubscriptionRole.Subscribes);
            requirements.Count().ShouldBe(2);
            requirements.ShouldContain(existing);
            requirements.ShouldContain(subscription);
        }

        [Test]
        public void save_a_new_subscription_with_a_mix_of_existing_subscriptions()
        {
            var existing = ObjectMother.ExistingSubscription();
            existing.NodeName = TheNodeName;

            persistence.Persist(existing);
            persistence.Persist(ObjectMother.ExistingSubscription("Different"));
            persistence.Persist(ObjectMother.ExistingSubscription("Different"));
            persistence.Persist(ObjectMother.ExistingSubscription("Different"));
            persistence.Persist(ObjectMother.ExistingSubscription("Different"));
            persistence.Persist(ObjectMother.ExistingSubscription("Different"));

            var subscription = ObjectMother.NewSubscription();
            subscription.NodeName = TheNodeName;

            theRepository.PersistSubscriptions(subscription);

            var requirements = theRepository.LoadSubscriptions(SubscriptionRole.Subscribes);
            requirements.Count().ShouldBe(2);
            requirements.ShouldContain(existing);
            requirements.ShouldContain(subscription);
        }

        [Test]
        public void save_a_subscription_that_already_exists()
        {
            var existing = ObjectMother.ExistingSubscription();
            existing.NodeName = TheNodeName;

            var subscription = existing.Clone();

            theRepository.PersistSubscriptions(subscription);

            theRepository.LoadSubscriptions(SubscriptionRole.Subscribes)
                .Single()
                .ShouldBe(existing);
        }

        [Test]
        public void save_a_mixed_bag_of_existing_and_new_subscriptions()
        {
            var existing = ObjectMother.ExistingSubscription(TheNodeName);

            var anotherExisting = ObjectMother.ExistingSubscription(TheNodeName);

            persistence.Persist(anotherExisting);
            persistence.Persist(existing);
            persistence.Persist(ObjectMother.ExistingSubscription("Different"));
            persistence.Persist(ObjectMother.ExistingSubscription("Different"));
            persistence.Persist(ObjectMother.ExistingSubscription("Different"));
            persistence.Persist(ObjectMother.ExistingSubscription("Different"));
            persistence.Persist(ObjectMother.ExistingSubscription("Different"));
            persistence.Persist(ObjectMother.ExistingSubscription("Different"));

            var old = existing.Clone();
            var newSubscription = ObjectMother.NewSubscription(TheNodeName);

            theRepository.PersistSubscriptions(old, newSubscription);
            var requirements = theRepository.LoadSubscriptions(SubscriptionRole.Subscribes);

            requirements.Count().ShouldBe(3);
            requirements.ShouldContain(existing);
            requirements.ShouldContain(newSubscription);
            requirements.ShouldContain(anotherExisting);


        }

        [Test]
        public void save_transport_node_for_the_first_time()
        {
            theRepository.Persist(new TransportNode(channelGraph));

            var node = persistence.NodesForGroup(channelGraph.Name)
                .Single();

            node.ShouldBe(new TransportNode(channelGraph));
            node.Id.ShouldNotBe(Guid.Empty.ToString());
        }

        [Test]
        public void saving_the_transport_node_is_idempotent()
        {
            theRepository.Persist(new TransportNode(channelGraph));

            var id = persistence.NodesForGroup(channelGraph.Name)
                .Single().Id;

            theRepository.Persist(new TransportNode(channelGraph));
            theRepository.Persist(new TransportNode(channelGraph));
            theRepository.Persist(new TransportNode(channelGraph));
            theRepository.Persist(new TransportNode(channelGraph));
            theRepository.Persist(new TransportNode(channelGraph));
            theRepository.Persist(new TransportNode(channelGraph));
            theRepository.Persist(new TransportNode(channelGraph));
            theRepository.Persist(new TransportNode(channelGraph));
            theRepository.Persist(new TransportNode(channelGraph));

            persistence.NodesForGroup(channelGraph.Name)
                .Single().Id.ShouldBe(id);
        }

        [Test]
        public void find_local()
        {
            var local = new TransportNode(channelGraph);

            theRepository.Persist(local, new TransportNode{Id="Foo"}, new TransportNode{Id = "Bar"});

            theRepository.FindLocal().ShouldBeTheSameAs(local);
        }

        [Test]
        public void find_peer()
        {
            var local = new TransportNode(channelGraph);

            var fooNode = new TransportNode { Id = "Foo" };
            theRepository.Persist(local, fooNode, new TransportNode { Id = "Bar" });

            theRepository.FindPeer("Foo")
                .ShouldBeTheSameAs(fooNode);
        }

        [Test]
        public void record_ownership_to_this_node_singular()
        {
            var local = new TransportNode(channelGraph);

            var fooNode = new TransportNode { Id = "Foo" };
            theRepository.Persist(local, fooNode, new TransportNode { Id = "Bar" });

            var subject = "foo://1".ToUri();

            theRepository.AddOwnershipToThisNode(subject);

            local.OwnedTasks.ShouldContain(subject);
        }

        [Test]
        public void record_multiple_ownerships_to_this_node()
        {
            var subjects = new Uri[] {"foo://1".ToUri(), "foo://2".ToUri(), "bar://1".ToUri()};

            var local = new TransportNode(channelGraph);

            var fooNode = new TransportNode { Id = "Foo" };
            theRepository.Persist(local, fooNode, new TransportNode { Id = "Bar" });

            theRepository.AddOwnershipToThisNode(subjects);

            local.OwnedTasks.ShouldHaveTheSameElementsAs(subjects);
        }

        [Test]
        public void remove_ownership_from_the_current_node()
        {
            var local = new TransportNode(channelGraph);

            var fooNode = new TransportNode { Id = "Foo" };
            theRepository.Persist(local, fooNode, new TransportNode { Id = "Bar" });

            var subject = "foo://1".ToUri();
            local.AddOwnership(subject);

            theRepository.RemoveOwnershipFromThisNode(subject);

            local.OwnedTasks.ShouldNotContain(subject);
        }

        [Test]
        public void remove_ownership_from_a_different_node()
        {
            var subject = "foo://1".ToUri();
            var local = new TransportNode(channelGraph);

            var fooNode = new TransportNode { Id = "Foo" };
            fooNode.AddOwnership(subject);

            theRepository.Persist(local, fooNode, new TransportNode { Id = "Bar" });


            theRepository.RemoveOwnershipFromNode(fooNode.Id, subject);

            fooNode.OwnedTasks.ShouldNotContain(subject);
        }

        [Test]
        public void remove_local_subscriptions()
        {
            var subscriptions = new[] { ObjectMother.NewSubscription(), ObjectMother.NewSubscription() };
            subscriptions.Each(x =>
            {
                x.Receiver = channelGraph.ReplyChannelFor("foo");
                x.Source = new Uri("foo://source");
            });

            theRepository.PersistSubscriptions(subscriptions);
            persistence.LoadSubscriptions(TheNodeName, SubscriptionRole.Subscribes)
                .ShouldHaveTheSameElementsAs(subscriptions);

            var removed = theRepository.RemoveLocalSubscriptions();
            removed.ShouldHaveTheSameElementsAs(subscriptions);

            persistence.LoadSubscriptions(TheNodeName, SubscriptionRole.Subscribes)
                .ShouldHaveCount(0);
        }

        [Test]
        public void remove_subscriptions_for_receiver()
        {
            var differentNode = ObjectMother.ExistingSubscription("DifferentNode");
            var differentReceiver = ObjectMother.ExistingSubscription();
            differentReceiver.Receiver = new Uri("memory://other_receiver");

            var subscriptions = new[]
            {
                ObjectMother.ExistingSubscription(),
                ObjectMother.ExistingSubscription(),
                ObjectMother.ExistingSubscription(),
                differentNode,
                differentReceiver
            };

            subscriptions.Each(x => x.Role = SubscriptionRole.Publishes);
            persistence.Persist(subscriptions);

            theRepository.RemoveSubscriptionsForReceiver(subscriptions[0].Receiver);

            persistence.LoadSubscriptions(TheNodeName, SubscriptionRole.Publishes)
                .ShouldHaveTheSameElementsAs(differentReceiver);
            persistence.LoadSubscriptions("DifferentNode", SubscriptionRole.Publishes)
                .ShouldHaveTheSameElementsAs(differentNode);
        }
    }
}