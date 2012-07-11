using System.Collections.Generic;
using System.Linq;
using FubuCore.Binding.InMemory;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Security;
using FubuMVC.Diagnostics.Chrome;
using FubuMVC.Diagnostics.Core.Configuration.Policies;
using FubuMVC.Diagnostics.Core.Infrastructure;
using FubuMVC.Diagnostics.Features.Html.Preview;
using FubuMVC.Diagnostics.Features.Html.Preview.Decorators;
using FubuMVC.Diagnostics.Navigation;
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

            Actions
                .IncludeType<BasicAssetDiagnostics>();


            Routes
                .UrlPolicy<DiagnosticsAttributeUrlPolicy>();

            Views
                .TryToAttachWithDefaultConventions()
                .RegisterActionLessViews(token => token.ViewModel == typeof (ChromeContent));

            Navigation<DiagnosticsMenu>();

            Configure(
                graph =>
                {
                    graph.Behaviors.Where(
                        x => x.ResourceType() != null && x.ResourceType().Name.ToLower().Contains("json")).Each(
                            x => x.MakeAsymmetricJson());
                });

        }

        private void setupDiagnosticServices()
        {
            Services(x =>
            {
                x.SetServiceIfNone<IBindingLogger, RecordingBindingLogger>();
                x.SetServiceIfNone<IDebugDetector, DebugDetector>();
                x.SetServiceIfNone<IDebugCallHandler, DebugCallHandler>();
                x.ReplaceService<IDebugReport, DebugReport>();
                x.ReplaceService<IDebugDetector, DebugDetector>();
                x.ReplaceService<IAuthorizationPolicyExecutor, RecordingAuthorizationPolicyExecutor>();
                x.ReplaceService<IBindingHistory, BindingHistory>();
                x.SetServiceIfNone<IRequestHistoryCache, RequestHistoryCache>();

                x.SetServiceIfNone<IPreviewModelActivator, PreviewModelActivator>();
                x.SetServiceIfNone<IPreviewModelTypeResolver, PreviewModelTypeResolver>();
                x.SetServiceIfNone<IPropertySourceGenerator, PropertySourceGenerator>();
                x.SetServiceIfNone<IModelPopulator, ModelPopulator>();
                x.SetServiceIfNone<ITagGeneratorFactory, TagGeneratorFactory>();

                x.SetServiceIfNone<IHtmlConventionsPreviewContextFactory, HtmlConventionsPreviewContextFactory>();

                x.ReplaceService<IDebugCallHandler, DiagnosticsDebugCallHandler>();

                x.Scan(scan =>
                {
                    scan
                        .Applies
                        .ToThisAssembly()
                        .ToAllPackageAssemblies();

                    scan
                        .AddAllTypesOf<IPreviewModelDecorator>();
                });
            });
        }
    }
}