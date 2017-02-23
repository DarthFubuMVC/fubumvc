using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.ServiceBus.Subscriptions;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Subscriptions
{
    
    public class SubscriptionHandler_is_registered_in_fubu_application_apps
    {
        [Fact]
        public void SubscriptionHandler_should_be_part_of_the_application()
        {
            using (var runtime = FubuRuntime.BasicBus())
            {
                var graph = runtime.Get<BehaviorGraph>();
                graph.ChainFor(typeof (SubscriptionRequested)).ShouldNotBeNull();
                graph.ChainFor(typeof (SubscriptionsChanged)).ShouldNotBeNull();
            }
        }
    }
}