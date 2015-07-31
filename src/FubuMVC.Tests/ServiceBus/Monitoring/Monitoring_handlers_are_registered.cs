using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Monitoring;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Monitoring
{
    [TestFixture]
    public class Monitoring_handlers_are_registered
    {
        [Test]
        public void handler_calls_are_registered_by_default()
        {
            using (var runtime = FubuTransport.DefaultPolicies())
            {
                runtime.Behaviors.ChainFor<TakeOwnershipRequest>().ShouldNotBeNull();
                runtime.Behaviors.ChainFor<TaskHealthRequest>().ShouldNotBeNull();
                runtime.Behaviors.ChainFor<TaskDeactivation>().ShouldNotBeNull();
            }

        }
    }
}