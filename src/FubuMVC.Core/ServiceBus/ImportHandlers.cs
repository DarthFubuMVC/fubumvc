using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core.Diagnostics.Packaging;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.ErrorHandling;
using FubuMVC.Core.ServiceBus.Monitoring;
using FubuMVC.Core.ServiceBus.Polling;
using FubuMVC.Core.ServiceBus.Registration.Nodes;
using FubuMVC.Core.ServiceBus.Subscriptions;

namespace FubuMVC.Core.ServiceBus
{
    // needs to be done AFTER authentication, so this is good
    public class ImportHandlers : IChainSource
    {
        public IEnumerable<BehaviorChain> BuildChains(BehaviorGraph graph, IPerfTimer timer)
        {
            return timer.Record("Building FubuTransportation Chains", () => buildChains(graph));
        }

        private static IEnumerable<BehaviorChain> buildChains(BehaviorGraph graph)
        {
            var handlers = graph.Settings.Get<HandlerGraph>();

            // TODO -- move this to a HandlerSource after we fix the duplicate calls
            // across HandlerSource problem.
            handlers.Add(HandlerCall.For<SubscriptionsHandler>(x => x.Handle(new SubscriptionRequested())));
            handlers.Add(HandlerCall.For<SubscriptionsHandler>(x => x.Handle(new SubscriptionsChanged())));
            handlers.Add(HandlerCall.For<SubscriptionsHandler>(x => x.Handle(new SubscriptionsRemoved())));
            handlers.Add(HandlerCall.For<MonitoringControlHandler>(x => x.Handle(new TakeOwnershipRequest())));
            handlers.Add(HandlerCall.For<MonitoringControlHandler>(x => x.Handle(new TaskHealthRequest())));
            handlers.Add(HandlerCall.For<MonitoringControlHandler>(x => x.Handle(new TaskDeactivation())));

            var jobs = graph.Settings.Get<PollingJobSettings>();
            jobs.Jobs.Each(x =>
            {
                var handlerType = typeof(JobRunner<>).MakeGenericType(x.JobType);
                var method = handlerType.GetMethod("Run");

                handlers.Add(new HandlerCall(handlerType, method)); ;
            });


            handlers.ApplyGeneralizedHandlers();

            var policies = graph.Settings.Get<HandlerPolicies>();
            handlers.ApplyPolicies(policies.GlobalPolicies);


            foreach (var chain in handlers)
            {
                // Apply the error handling node
                chain.InsertFirst(new ExceptionHandlerNode(chain));

                // Hate how we're doing this, but disable tracing
                // on the polling job requests here.
                if (chain.InputType().Closes(typeof (JobRequest<>)))
                {
                    chain.Tags.Add(BehaviorChain.NoTracing);
                }
            }

            return handlers;
        }
    }
}