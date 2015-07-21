using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.InMemory;
using FubuMVC.Core.ServiceBus.Subscriptions;
using FubuTransportation.LightningQueues;

namespace FubuTransportation.Storyteller.Fixtures
{
    public class RunningNode
    {
        private readonly Uri _replyUri;

        public static readonly HarnessSettings Settings
            = InMemoryTransport.ToInMemory<HarnessSettings>();

        public static Cache<string, InMemorySubscriptionPersistence> Subscriptions =
            new Cache<string, InMemorySubscriptionPersistence>(name => new InMemorySubscriptionPersistence());

        private readonly Type _registryType;
        private readonly string _contents;
        private FubuRuntime _runtime;
        private InMemorySubscriptionPersistence _persistence;

        public string Name { get; set; }

        public RunningNode(string typeName, Uri replyUri)
        {
            _replyUri = replyUri;


            _registryType =
                Assembly.GetExecutingAssembly()
                    .ExportedTypes.Where(x => x.IsConcreteTypeOf<FubuTransportRegistry>())
                    .FirstOrDefault(x => x.Name.EqualsIgnoreCase(typeName));

            var file =
                new FileSystem().FindFiles(Environment.CurrentDirectory.ParentDirectory().ParentDirectory(),
                    FileSet.Deep(_registryType.Name + ".cs")).FirstOrDefault();

            if (file != null)
            {
                _contents = new FileSystem().ReadStringFromFile(file);
            }
        }

        public void Start()
        {
            var registry = Activator.CreateInstance(_registryType).As<FubuTransportRegistry>();
            registry.Services(x =>
            {
                _persistence = Subscriptions[registry.NodeName];
                x.ReplaceService<ISubscriptionPersistence>(_persistence);
                x.ReplaceService(Settings);
            });

            registry.AlterSettings<LightningQueueSettings>(x => x.Disabled = true);

            registry.EnableInMemoryTransport(_replyUri);

            _runtime = FubuTransport.For(registry).Bootstrap();
        }

        public InMemorySubscriptionPersistence Persistence
        {
            get { return _persistence; }
        }

        public IEnumerable<Subscription> LoadedSubscriptions()
        {
            return _runtime.Factory.Get<ISubscriptionCache>()
                .ActiveSubscriptions;
        }

        public IEnumerable<Subscription> PersistedSubscriptions(SubscriptionRole role = SubscriptionRole.Publishes)
        {
            return _runtime.Factory.Get<ISubscriptionRepository>().LoadSubscriptions(role);
        }


        public void Dispose()
        {
            _runtime.Dispose();
        }

        public string Contents
        {
            get { return _contents; }
        }

        public IEnumerable<TransportNode> PersistedNodes()
        {
            return _runtime.Factory.Get<ISubscriptionPersistence>()
                .NodesForGroup(_runtime.Factory.Get<ChannelGraph>().Name);
        }

        public void RemoveSubscriptions()
        {
            _runtime.Factory.Get<IServiceBus>().RemoveSubscriptionsForThisNodeAsync().Wait();
        }
    }
}