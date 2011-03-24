using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Diagnostics.Behaviors;
using FubuMVC.Diagnostics.Configuration;
using FubuMVC.Diagnostics.Configuration.SparkPolicies;
using FubuMVC.Diagnostics.Endpoints;
using FubuMVC.Diagnostics.Grids;
using FubuMVC.Diagnostics.Grids.Builders;
using FubuMVC.Diagnostics.Grids.Columns;
using FubuMVC.Diagnostics.Grids.Filters;
using FubuMVC.Diagnostics.Infrastructure;
using FubuMVC.Diagnostics.Models;
using FubuMVC.Diagnostics.Navigation;
using FubuMVC.Diagnostics.Notifications;
using FubuMVC.Diagnostics.Partials;
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
                                    .Add<PartialActionSparkPolicy>()
									.Add<NotificationActionSparkPolicy>());

            Services(x =>
                         {
                             // Typically you'd do this in your container but we're keeping this IoC-agnostic
                             x.SetServiceIfNone<IHttpRequest, HttpRequest>();
                             x.SetServiceIfNone<IHttpConstraintResolver, HttpConstraintResolver>();
                             x.SetServiceIfNone<INavigationMenuBuilder, NavigationMenuBuilder>();
                             x.SetServiceIfNone<IAuthorizationDescriptor, AuthorizationDescriptor>();
                             x.SetServiceIfNone(typeof(IGridService<>), typeof(GridService<>));
                             x.SetServiceIfNone<IGridRowBuilder<BehaviorGraph>, BehaviorGraphGridRowBuilder>();
                             x.AddService(typeof(IPartialModel), new ObjectDef { Type = typeof(NavigationMenu) });
                             x.AddService(typeof(IPartialDecorator<NavigationMenu>), new ObjectDef { Type = typeof(NavigationMenuDecorator) });
                             x.AddService(typeof(INavigationItemAction), new ObjectDef { Type = typeof(DashboardAction) });
                             x.AddService(typeof(INavigationItemAction), new ObjectDef { Type = typeof(RouteExplorerAction) });
                             x.AddService(typeof(INavigationItemAction), new ObjectDef { Type = typeof(RouteAuthorizationAction) });
                             x.AddService(typeof(INavigationItemAction), new ObjectDef { Type = typeof(PackageDiagnosticsAction) });
                             x.AddService(typeof(IGridColumnBuilder<>), new ObjectDef { Type = typeof(DefaultBehaviorChainColumnBuilder) });

							 x.AddService(typeof(INotificationPolicy), new ObjectDef { Type = typeof(NoOutputsNotificationPolicy)});

							 // TODO -- a scanning mechanism for registering these would be nice
							 x.AddService(typeof(IBehaviorChainColumn), new ObjectDef { Type = typeof(RouteColumn) });
							 x.AddService(typeof(IBehaviorChainColumn), new ObjectDef { Type = typeof(ConstraintsColumn) });
							 x.AddService(typeof(IBehaviorChainColumn), new ObjectDef { Type = typeof(ActionColumn) });
							 x.AddService(typeof(IBehaviorChainColumn), new ObjectDef { Type = typeof(InputModelColumn) });
							 x.AddService(typeof(IBehaviorChainColumn), new ObjectDef { Type = typeof(OutputModelColumn) });
							 x.AddService(typeof(IBehaviorChainColumn), new ObjectDef { Type = typeof(ChainUrlColumn) });
							 x.AddService(typeof(IBehaviorChainColumn), new ObjectDef { Type = typeof(UrlCategoryColumn) });
							 x.AddService(typeof(IBehaviorChainColumn), new ObjectDef { Type = typeof(OriginColumn) });
							 x.AddService(typeof(IBehaviorChainColumn), new ObjectDef { Type = typeof(ViewColumn) });

							 // TODO -- a scanning mechanism for registering these would be nice
                             x.AddService(typeof(IGridFilter<BehaviorChain>), new ObjectDef { Type = typeof(ConstraintsFilter) });
                             x.AddService(typeof(IGridFilter<BehaviorChain>), new ObjectDef { Type = typeof(InputModelFilter) });
                             x.AddService(typeof(IGridFilter<BehaviorChain>), new ObjectDef { Type = typeof(OutputModelFilter) });
                             x.AddService(typeof(IGridFilter<BehaviorChain>), new ObjectDef { Type = typeof(OriginFilter) });
                             x.AddService(typeof(IGridFilter<BehaviorChain>), new ObjectDef { Type = typeof(RouteFilter) });
                             x.AddService(typeof(IGridFilter<BehaviorChain>), new ObjectDef { Type = typeof(UrlCategoryFilter) });
                             x.AddService(typeof(IGridFilter<BehaviorChain>), new ObjectDef { Type = typeof(AuthorizationFilter) });
							 x.AddService(typeof(IGridFilter<BehaviorChain>), new ObjectDef { Type = typeof(ViewFilter) });
                         });

        	Policies
        		.ConditionallyWrapBehaviorChainsWith<UnknownChainBehavior>(
        			call => call.InputType() == typeof (ChainRequest));

            Actions
                .FindWith<PartialActionSource>()
				.FindWith<NotificationActionSource>();

            Output
                .ToJson
                .WhenCallMatches(call => call.OutputType().Name.StartsWith("Json"));
        }
    }
}