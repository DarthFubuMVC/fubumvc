using System.Web.UI;
using FubuMVC.Core.StructureMap;
using FubuTestingSupport;
using FubuTransportation.Configuration;
using FubuTransportation.Subscriptions;
using NUnit.Framework;

namespace FubuTransportation.Testing.Subscriptions
{
    [TestFixture]
    public class SubscriptionHandler_is_registered_in_fubu_application_apps
    {
        [Test]
        public void SubscriptionHandler_should_be_part_of_the_application()
        {
            using (var runtime = FubuTransport.DefaultPolicies().Bootstrap())
            {
                var graph = runtime.Factory.Get<HandlerGraph>();
                graph.ChainFor(typeof (SubscriptionRequested)).ShouldNotBeNull();
                graph.ChainFor(typeof (SubscriptionsChanged)).ShouldNotBeNull();
            }
        }
    }
}