using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Diagnostics.Behaviors;
using FubuMVC.Diagnostics.Configuration;
using FubuMVC.Diagnostics.Configuration.Partials;
using FubuMVC.Diagnostics.Configuration.SparkPolicies;
using FubuMVC.Diagnostics.Endpoints;
using FubuMVC.Diagnostics.Infrastructure;
using FubuMVC.Diagnostics.Infrastructure.Grids;
using FubuMVC.Diagnostics.Infrastructure.Grids.Builders;
using FubuMVC.Diagnostics.Models;
using FubuMVC.Diagnostics.Models.Grids;
using FubuMVC.Diagnostics.Navigation;
using Spark.Web.FubuMVC;

namespace FubuMVC.Diagnostics
{
    public class FubuDiagnosticsRegistry : FubuPackageRegistry
    {
        public FubuDiagnosticsRegistry()
        {
            this.ApplyEndpointConventions(typeof(DiagnosticsEndpointMarker));
            this.Spark(spark => spark
                                    .Policies
                                    .Add(new DiagnosticsEndpointSparkPolicy(typeof (DiagnosticsEndpointMarker)))
                                    .Add<PartialActionSparkPolicy>());

            Services(x =>
                         {
                             // Typically you'd do this in your container but we're keeping this IoC-agnostic
                             x.SetServiceIfNone<IHttpRequest, HttpRequest>();
                             x.SetServiceIfNone<IHttpConstraintResolver, HttpConstraintResolver>();
                             x.SetServiceIfNone<INavigationMenuBuilder, NavigationMenuBuilder>();
                             x.SetServiceIfNone(typeof(IGridService<>), typeof(GridService<>));
                             x.SetServiceIfNone<IGridRowBuilder<BehaviorGraph>, BehaviorGraphGridRowBuilder>();
                             x.AddService(typeof(IPartialModel), new ObjectDef { Type = typeof(NavigationMenu) });
                             x.AddService(typeof(IPartialDecorator<NavigationMenu>), new ObjectDef { Type = typeof(NavigationMenuDecorator) });
                             x.AddService(typeof(INavigationItemAction), new ObjectDef { Type = typeof(DashboardAction) });
                             x.AddService(typeof(INavigationItemAction), new ObjectDef { Type = typeof(RouteExplorerAction) });
                             x.AddService(typeof(IGridColumnBuilder<>), new ObjectDef { Type = typeof(DefaultGridColumnBuilder) });
                         });

        	Policies
        		.ConditionallyWrapBehaviorChainsWith<UnknownChainBehavior>(
        			call => call.InputType() == typeof (UnknownChainRequest));

            Actions
                .FindWith<PartialActionSource>();

            Output
                .ToJson
                .WhenTheOutputModelIs<JsonGridModel>();
        }
    }
}