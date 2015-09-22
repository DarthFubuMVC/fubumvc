using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FubuMVC.Core.Diagnostics.Packaging;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Monitoring;
using FubuMVC.Core.ServiceBus.Polling;
using FubuMVC.Core.ServiceBus.Registration.Nodes;
using FubuMVC.Core.ServiceBus.Subscriptions;

namespace FubuMVC.Core.ServiceBus
{
    // needs to be done AFTER authentication, so this is good
    public class SystemLevelHandlers : IChainSource
    {
        public Task<BehaviorChain[]> BuildChains(BehaviorGraph graph, IPerfTimer timer)
        {
            return Task.Factory.StartNew(() =>
            {
                return timer.Record("Building System Level Handler Chains", () => buildChains(graph).ToArray());
            });
        }

        private static IEnumerable<BehaviorChain> buildChains(BehaviorGraph graph)
        {
            var handlers = new HandlerGraph
            {
                HandlerCall.For<SubscriptionsHandler>(x => x.Handle(new SubscriptionRequested())),
                HandlerCall.For<SubscriptionsHandler>(x => x.Handle(new SubscriptionsChanged())),
                HandlerCall.For<SubscriptionsHandler>(x => x.Handle(new SubscriptionsRemoved())),
                HandlerCall.For<MonitoringControlHandler>(x => x.Handle(new TakeOwnershipRequest())),
                HandlerCall.For<MonitoringControlHandler>(x => x.Handle(new TaskHealthRequest())),
                HandlerCall.For<MonitoringControlHandler>(x => x.Handle(new TaskDeactivation()))
            };


            handlers.Compile();

            // TODO -- need to apply [ModifyChainAttribute]'s here. See GH-958

            return handlers;
        }
    }
}