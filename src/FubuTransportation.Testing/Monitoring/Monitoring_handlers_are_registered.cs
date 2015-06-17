using FubuMVC.StructureMap;
using FubuTestingSupport;
using FubuTransportation.Configuration;
using FubuTransportation.Monitoring;
using NUnit.Framework;

namespace FubuTransportation.Testing.Monitoring
{
    [TestFixture]
    public class Monitoring_handlers_are_registered
    {
        [Test]
        public void handler_calls_are_registered_by_default()
        {
            var graph = FubuTransportRegistry.HandlerGraphFor(x => { });

            graph.ChainFor<TakeOwnershipRequest>().ShouldNotBeNull();
            graph.ChainFor<TaskHealthRequest>().ShouldNotBeNull();
            graph.ChainFor<TaskDeactivation>().ShouldNotBeNull();
        }
    }
}