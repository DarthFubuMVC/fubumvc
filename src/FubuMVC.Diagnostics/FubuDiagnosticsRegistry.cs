using System;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Behaviors;
using FubuMVC.Diagnostics.Configuration;
using FubuMVC.Diagnostics.Endpoints;
using FubuMVC.Diagnostics.Grids;
using FubuMVC.Diagnostics.Grids.Columns;
using FubuMVC.Diagnostics.Infrastructure;
using FubuMVC.Diagnostics.Models;
using FubuMVC.Diagnostics.Models.Requests;
using FubuMVC.Diagnostics.Navigation;
using FubuMVC.Diagnostics.Notifications;
using FubuMVC.Diagnostics.Partials;
using FubuMVC.Spark;

namespace FubuMVC.Diagnostics
{
    public class FubuDiagnosticsRegistry : FubuPackageRegistry
    {
        public FubuDiagnosticsRegistry()
        {
            Applies
                .ToAssemblyContainingType<BehaviorStart>();

            this.ApplyEndpointConventions(typeof(DiagnosticsEndpointMarker));

            Services(x =>
                         {
                             // Typically you'd do this in your container but we're keeping this IoC-agnostic
                             x.SetServiceIfNone<IHttpRequest, HttpRequest>();
                             x.SetServiceIfNone<IHttpConstraintResolver, HttpConstraintResolver>();
							 x.SetServiceIfNone<IRequestCacheModelBuilder, RequestCacheModelBuilder>();
                             x.SetServiceIfNone<INavigationMenuBuilder, NavigationMenuBuilder>();
                             x.SetServiceIfNone<IAuthorizationDescriptor, AuthorizationDescriptor>();
                             x.SetServiceIfNone(typeof(IGridService<,>), typeof(GridService<,>));
                             x.SetServiceIfNone(typeof(IGridRowBuilder<,>), typeof(GridRowBuilder<,>));
							 x.SetServiceIfNone(typeof(IGridColumnBuilder<>), typeof(GridColumnBuilder<>));
							 x.SetServiceIfNone<IGridRowProvider<BehaviorGraph, BehaviorChain>, BehaviorGraphRowProvider>();
							 x.SetServiceIfNone<IGridRowProvider<RequestCacheModel, RecordedRequestModel>, RequestCacheRowProvider>();

							 x.Scan(scan =>
							        	{
							        		scan
												.Applies
												.ToThisAssembly()
												.ToAllPackageAssemblies();

											scan
												.AddAllTypesOf<INavigationItemAction>()
												.AddAllTypesOf<INotificationPolicy>();

							        		scan
							        			.ConnectImplementationsToTypesClosing(typeof (IPartialDecorator<>))
							        			.ConnectImplementationsToTypesClosing(typeof (IGridColumnBuilder<>))
												.ConnectImplementationsToTypesClosing(typeof(IGridColumn<>))
												.ConnectImplementationsToTypesClosing(typeof(IGridFilter<>));
							        	});
                         });



            Policies
                .ConditionallyWrapBehaviorChainsWith<UnknownObjectBehavior>(call => call.InputType() == typeof (ChainRequest));

            Actions
                .FindWith<PartialActionSource>()
				.FindWith<NotificationActionSource>();

            Views
                .TryToAttachWithDefaultConventions()
                .RegisterActionLessViews(token => token.ViewModelType.IsDiagnosticsReport());

            this.UseSpark();

            Output
                .ToJson
                .WhenCallMatches(call => call.OutputType().Name.StartsWith("Json"));
        }
    }

    public static class DiagnosticsExtensions
    {
        public static bool IsDiagnosticsReport(this Type type)
        {
            return typeof (IBehaviorDetails).IsAssignableFrom(type) ||
                   typeof (IModelBindingDetail).IsAssignableFrom(type);
        }
    }
}