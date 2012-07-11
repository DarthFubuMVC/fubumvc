using System.Collections.Generic;
using System.Linq;
using FubuCore.Binding.InMemory;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security;
using FubuMVC.Core.UI.Security;
using FubuMVC.Diagnostics.Chrome;
using FubuMVC.Diagnostics.Core.Configuration.Policies;
using FubuMVC.Diagnostics.Core.Grids;
using FubuMVC.Diagnostics.Core.Grids.Columns;
using FubuMVC.Diagnostics.Core.Infrastructure;
using FubuMVC.Diagnostics.Features;
using FubuMVC.Diagnostics.Features.Chains.View;
using FubuMVC.Diagnostics.Features.Html.Preview;
using FubuMVC.Diagnostics.Features.Html.Preview.Decorators;
using FubuMVC.Diagnostics.Features.Requests;
using FubuMVC.Diagnostics.Models;
using FubuMVC.Diagnostics.Models.Grids;
using FubuMVC.Diagnostics.Notifications;
using FubuMVC.Diagnostics.Partials;
using FubuMVC.Diagnostics.Runtime;
using FubuMVC.Diagnostics.Runtime.Assets;
using FubuMVC.Diagnostics.Runtime.Tracing;

namespace FubuMVC.Diagnostics
{
    public class AdvancedDiagnosticsRegistry : FubuRegistry
    {
        public AdvancedDiagnosticsRegistry()
        {
            setupDiagnosticServices();

            Applies
                .ToAssemblyContainingType<AdvancedDiagnosticsRegistry>()
                .ToAssemblyContainingType<BehaviorStart>()
                .ToAllPackageAssemblies();

            ApplyHandlerConventions(markers => new DiagnosticsHandlerUrlPolicy(markers), typeof (DiagnosticsFeatures));

            Actions
                .IncludeType<BasicAssetDiagnostics>();


            Routes
                .UrlPolicy<DiagnosticsAttributeUrlPolicy>();

            Views
                .TryToAttachWithDefaultConventions()
                .RegisterActionLessViews(token => typeof (IPartialModel).IsAssignableFrom(token.ViewModel))
                .RegisterActionLessViews(token => token.ViewModel == typeof (ChromeContent));

            Navigation<DiagnosticsMenu>();

            Configure(
                graph =>
                {
                    graph.Behaviors.Where(
                        x => x.ResourceType() != null && x.ResourceType().Name.ToLower().Contains("json")).Each(
                            x => x.MakeAsymmetricJson());
                });

            Models.IgnoreProperties(prop => prop.PropertyType == typeof (IEnumerable<JsonGridFilter>));
        }

        private void setupDiagnosticServices()
        {
            Services(x =>
            {
                x.SetServiceIfNone<IBindingLogger, RecordingBindingLogger>();
                x.SetServiceIfNone<IDebugDetector, DebugDetector>();
                x.SetServiceIfNone<IDebugCallHandler, DebugCallHandler>();
                x.ReplaceService<IDebugReport, DebugReport>();
                x.ReplaceService<IFubuRequest, RecordingFubuRequest>();
                x.ReplaceService<IDebugDetector, DebugDetector>();
                x.ReplaceService<IAuthorizationPolicyExecutor, RecordingAuthorizationPolicyExecutor>();
                x.ReplaceService<IOutputWriter, RecordingOutputWriter>();
                x.ReplaceService<IBindingHistory, BindingHistory>();
                x.SetServiceIfNone<IRequestHistoryCache, RequestHistoryCache>();

                // TODO -- need to test this
                x.ReplaceService<IFieldAccessRightsExecutor, RecordingFieldAccessRightsExecutor>();

                // Typically you'd do this in your container but we're keeping this IoC-agnostic
                x.SetServiceIfNone<IHttpConstraintResolver, HttpConstraintResolver>();
                x.SetServiceIfNone<IAuthorizationDescriptor, AuthorizationDescriptor>();
                x.SetServiceIfNone(typeof (IGridService<,>), typeof (GridService<,>));
                x.SetServiceIfNone(typeof (IGridRowBuilder<,>), typeof (GridRowBuilder<,>));
                x.SetServiceIfNone(typeof (IGridColumnBuilder<>), typeof (GridColumnBuilder<>));
                x.SetServiceIfNone
                    <IGridRowProvider<BehaviorGraph, BehaviorChain>, BehaviorGraphRowProvider>();
                x.SetServiceIfNone<IPreviewModelActivator, PreviewModelActivator>();
                x.SetServiceIfNone<IPreviewModelTypeResolver, PreviewModelTypeResolver>();
                x.SetServiceIfNone<IPropertySourceGenerator, PropertySourceGenerator>();
                x.SetServiceIfNone<IModelPopulator, ModelPopulator>();
                x.SetServiceIfNone<ITagGeneratorFactory, TagGeneratorFactory>();
                x.SetServiceIfNone<IJsonProvider, JsonProvider>();
                x.SetServiceIfNone<IChainVisualizerBuilder,
                    ChainVisualizerBuilder>();

                x.ReplaceService<IDebugCallHandler, DiagnosticsDebugCallHandler>();

                x.Scan(scan =>
                {
                    scan
                        .Applies
                        .ToThisAssembly()
                        .ToAllPackageAssemblies();

                    scan
                        .AddAllTypesOf<INotificationPolicy>()
                        .AddAllTypesOf<IPreviewModelDecorator>();

                    scan
                        .ConnectImplementationsToTypesClosing(typeof (IPartialDecorator<>))
                        .ConnectImplementationsToTypesClosing(typeof (IGridColumnBuilder<>))
                        .ConnectImplementationsToTypesClosing(typeof (IGridColumn<>))
                        .ConnectImplementationsToTypesClosing(typeof (IGridFilter<>))
                        .ConnectImplementationsToTypesClosing(typeof (IModelBuilder<>));
                });
            });
        }
    }
}