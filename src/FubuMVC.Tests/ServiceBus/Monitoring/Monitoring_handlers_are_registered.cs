using FubuMVC.Core;
using FubuMVC.Core.ServiceBus.Monitoring;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Monitoring
{
    
    public class Monitoring_handlers_are_registered
    {
        [Fact]
        public void handler_calls_are_registered_by_default()
        {
            using (var runtime = FubuRuntime.BasicBus())
            {
                runtime.Behaviors.ChainFor<TakeOwnershipRequest>().ShouldNotBeNull();
                runtime.Behaviors.ChainFor<TaskHealthRequest>().ShouldNotBeNull();
                runtime.Behaviors.ChainFor<TaskDeactivation>().ShouldNotBeNull();
            }
        }
    }
}