using System.Collections.Generic;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.GettingStarted.Intro;
using FubuMVC.Spark;

namespace FubuMVC.GettingStarted
{
    public class GettingStartedExtension : IFubuRegistryExtension
    {
        public void Configure(FubuRegistry registry)
        {
            registry
                .Actions
                .FindWith<ExplicitControllerRegistration>();

            registry
                .Views
                .TryToAttachWithDefaultConventions()
                .TryToAttachViewsInPackages();

            registry
                .UseSpark(spark => spark.ConfigureComposer(c => c.AddBinder<DiagnosticsTemplateBinder>()));

            registry.Configure(graph =>
                                   {
                                       var chain = graph.FindHomeChain();
                                       if (chain == null)
                                       {
                                           graph
                                               .BehaviorFor<GettingStartedController>(x => x.Execute(new GettingStartedRequestModel()))
                                               .Route = new RouteDefinition(string.Empty);
                                       }
                                   });
        }

        public class ExplicitControllerRegistration : IActionSource
        {
            public IEnumerable<ActionCall> FindActions(TypePool types)
            {
                yield return ActionCall.For<GettingStartedController>(c => c.Execute(new GettingStartedRequestModel()));
            }
        }
    }
}