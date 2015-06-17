using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuTestingSupport;
using FubuTransportation.Runtime;
using FubuTransportation.Runtime.Delayed;
using LightningQueues.Model;
using NUnit.Framework;

namespace FubuTransportation.LightningQueues.Testing
{
    //TODO test failures under resharper
    [TestFixture]
    public class LightningQueuesServiceRegistry_spec
    {
        private void registeredTypeIs<TService, TImplementation>()
        {
            var registry = new FubuRegistry();
            registry.Services<LightningQueuesServiceRegistry>();
            BehaviorGraph.BuildFrom(registry).Services.DefaultServiceFor<TService>().Type.ShouldEqual(
                typeof(TImplementation));
        }

        [Test]
        public void LightningQueuesTransport_is_registered()
        {
            registeredTypeIs<ITransport, LightningQueuesTransport>();
        }

        [Test]
        public void PersistentQueue_is_registered()
        {
            registeredTypeIs<IPersistentQueues, PersistentQueues>();
        }

        [Test]
        public void IDelayedMessageCacheMessageId_is_registered()
        {
            registeredTypeIs<IDelayedMessageCache<MessageId>, DelayedMessageCache<MessageId>>();

            ServiceRegistry.ShouldBeSingleton(typeof(DelayedMessageCache<MessageId>))
                .ShouldBeTrue();
        }

    }
}