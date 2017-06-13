using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Subscriptions;
using FubuMVC.RavenDb.ServiceBus;
using Shouldly;
using StructureMap;
using Xunit;

namespace FubuMVC.RavenDb.Tests.ServiceBus
{
    public class RavenDbSubscriptionPersistenceTester : IDisposable
    {
        private FubuRuntime runtime;
        private RavenDbSubscriptionPersistence persistence;

        public RavenDbSubscriptionPersistenceTester()
        {
            runtime = FubuRuntime.BasicBus();
            runtime.Get<IContainer>().UseInMemoryDatastore();

            persistence = runtime.Get<RavenDbSubscriptionPersistence>();
        }

        [Fact]
        public void persist_and_load_all_new_subscriptions()
        {
            var subscriptions = new[]
            {
                ExistingSubscription("Node1"),
                ExistingSubscription("Node1"),
                ExistingSubscription("Node1"),
                ExistingSubscription("Node1"),
                ExistingSubscription("Node1")
            };

            var subscriptions2 = new[]
            {
                ExistingSubscription("Node2"),
                ExistingSubscription("Node2")
            };

            persistence.Persist(subscriptions);
            persistence.Persist(subscriptions2);

            var loaded = persistence.LoadSubscriptions("Node1", SubscriptionRole.Subscribes);
            loaded.ShouldHaveTheSameElementsAs(subscriptions);

            persistence.LoadSubscriptions("Node2", SubscriptionRole.Subscribes)
                .ShouldHaveTheSameElementsAs(subscriptions2);
        }

        [Fact]
        public void persists_and_loads_large_number_of_subscriptions()
        {
            var subscriptions = new List<Subscription>();
            Enumerable.Range(0, 2000).Each(_ => subscriptions.Add(ExistingSubscription("Node1")));

            persistence.Persist(subscriptions);

            var loaded = persistence.LoadSubscriptions("Node1", SubscriptionRole.Subscribes);
            loaded.ShouldHaveTheSameElementsAs(subscriptions);
        }

        [Fact]
        public void persist_and_load_transport_nodes()
        {
            var node1 = new TransportNode
            {
                Id = Guid.NewGuid().ToString(),
                MachineName = "Box1",
                NodeName = "Node1",
                Addresses = new Uri[] {"memory://1".ToUri(), "memory://2".ToUri()}
            };

            persistence.Persist(node1);

            var node2 = new TransportNode
            {
                Id = Guid.NewGuid().ToString(),
                MachineName = "Box2",
                NodeName = "Node2",
                Addresses = new Uri[] {"memory://3".ToUri(), "memory://4".ToUri()}
            };

            persistence.Persist(node2);

            persistence.NodesForGroup("Node1").Single()
                .ShouldBe(node1);

            persistence.NodesForGroup("Node2").Single()
                .ShouldBe(node2);
        }

        [Fact]
        public void load_a_specific_node()
        {
            var node1 = new TransportNode
            {
                Id = Guid.NewGuid().ToString(),
                MachineName = "Box1",
                NodeName = "Node1",
                Addresses = new Uri[] {"memory://1".ToUri(), "memory://2".ToUri()}
            };


            var node2 = new TransportNode
            {
                Id = Guid.NewGuid().ToString(),
                MachineName = "Box2",
                NodeName = "Node2",
                Addresses = new Uri[] {"memory://3".ToUri(), "memory://4".ToUri()}
            };

            persistence.Persist(node1, node2);

            persistence.LoadNode(node2.Id)
                .MachineName.ShouldBe(node2.MachineName);

            persistence.LoadNode(node1.Id)
                .MachineName.ShouldBe(node1.MachineName);
        }

        [Fact]
        public void alter_a_node()
        {
            var node1 = new TransportNode
            {
                Id = Guid.NewGuid().ToString(),
                MachineName = "Box1",
                NodeName = "Node1",
                Addresses = new Uri[] {"memory://1".ToUri(), "memory://2".ToUri()}
            };


            var node2 = new TransportNode
            {
                Id = Guid.NewGuid().ToString(),
                MachineName = "Box2",
                NodeName = "Node2",
                Addresses = new Uri[] {"memory://3".ToUri(), "memory://4".ToUri()}
            };

            var subject = "foo://1".ToUri();

            persistence.Persist(node1, node2);


            persistence.Alter(node1.Id, x => x.AddOwnership(subject));

            persistence.LoadNode(node1.Id).OwnedTasks
                .ShouldContain(subject);
        }

        [Fact]
        public void delete_susbscriptions()
        {
            var subscriptions = new Subscription[]
            {
                ExistingSubscription("Node1"),
                ExistingSubscription("Node2"),
                ExistingSubscription("Node2")
            };

            persistence.Persist(subscriptions);

            persistence.DeleteSubscriptions(subscriptions.Take(2));

            persistence.LoadSubscriptions("Node1", SubscriptionRole.Subscribes)
                .ShouldHaveCount(0);
            persistence.LoadSubscriptions("Node2", SubscriptionRole.Subscribes)
                .ShouldHaveTheSameElementsAs(subscriptions.Skip(2));
        }

        public static Subscription ExistingSubscription(string nodeName = null)
        {
            var subscription = new Subscription
            {
                MessageType = Guid.NewGuid().ToString(),
                NodeName = nodeName ?? "TheNode",
                Receiver = "memory://receiver".ToUri(),
                Source = "memory://source".ToUri(),
                Role = SubscriptionRole.Subscribes

            };
            subscription.Id = Guid.NewGuid();

            if (nodeName.IsNotEmpty())
            {
                subscription.NodeName = nodeName;
            }

            return subscription;
        }

        public void Dispose()
        {
            runtime?.Dispose();
        }
    }
}
