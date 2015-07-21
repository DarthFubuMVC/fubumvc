using System.Linq;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.InMemory;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuTransportation.Testing.InMemory
{
    [TestFixture]
    public class InMemoryTransport_opt_in_registration_tester
    {
        [Test]
        public void is_not_registered_normally()
        {
            FubuTransport.Reset();

            var graph = FubuTransportRegistry.BehaviorGraphFor(x => { });
            graph.Services.ServicesFor<ITransport>()
                .Any(x => x.Type == typeof(InMemoryTransport))
                .ShouldBeFalse();
        }

        [Test]
        public void is_registered_if_FubuTransport_in_memory_testing()
        {
            FubuTransport.AllQueuesInMemory = true;

            var graph = FubuTransportRegistry.BehaviorGraphFor(x => { });
            graph.Services.ServicesFor<ITransport>()
                .Any(x => x.Type == typeof(InMemoryTransport))
                .ShouldBeTrue();
        }

        [Test]
        public void is_registered_if_user_opted_into_in_memory_transport()
        {
            FubuTransport.Reset();

            var graph = FubuTransportRegistry.BehaviorGraphFor(x => {
                x.AlterSettings<TransportSettings>(o => o.EnableInMemoryTransport = true);
            });
            graph.Services.ServicesFor<ITransport>()
                .Any(x => x.Type == typeof(InMemoryTransport))
                .ShouldBeTrue();
        }
    }
}