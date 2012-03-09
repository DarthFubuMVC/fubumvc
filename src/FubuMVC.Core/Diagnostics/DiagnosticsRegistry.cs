using System.Collections.Generic;
using FubuCore.Binding;
using FubuCore.Binding.InMemory;
using FubuCore.Reflection;
using FubuMVC.Core.Diagnostics.Assets;
using FubuMVC.Core.Diagnostics.HtmlWriting;
using FubuMVC.Core.Diagnostics.Querying;
using FubuMVC.Core.Diagnostics.Tracing;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security;
using FubuMVC.Core.UI.Diagnostics;
using FubuMVC.Core.UI.Security;

namespace FubuMVC.Core.Diagnostics
{
    public class DiagnosticsRegistry : FubuRegistry
    {
        public DiagnosticsRegistry()
        {
            Applies
                .ToAssemblyContainingType<DiagnosticsRegistry>();

            Actions
                .IncludeTypes(x => x.HasAttribute<FubuDiagnosticsAttribute>())
                .IncludeType<GraphQuery>()
                .IncludeType<BasicAssetDiagnostics>()
                .IncludeType<ScriptWriter>();

            Routes
                .UrlPolicy<DiagnosticUrlPolicy>();


            Services(x =>
            {
                // TODO -- need to register IBindingHistory
                x.SetServiceIfNone<IBindingLogger, RecordingBindingLogger>();
                x.SetServiceIfNone<IDebugDetector, DebugDetector>();
                x.SetServiceIfNone<IDebugCallHandler, DebugCallHandler>();
                x.ReplaceService<IDebugReport, DebugReport>();
                x.ReplaceService<IRequestObserver, RequestObserver>();
                x.ReplaceService<IFubuRequest, RecordingFubuRequest>();
                x.ReplaceService<IDebugDetector, DebugDetector>();
                x.ReplaceService<IAuthorizationPolicyExecutor, RecordingAuthorizationPolicyExecutor>();
                x.ReplaceService<IOutputWriter, RecordingOutputWriter>();
                x.ReplaceService<IBindingHistory, BindingHistory>();
                x.SetServiceIfNone<IRequestHistoryCache, RequestHistoryCache>();

                // TODO -- need to test this
                x.ReplaceService<IFieldAccessRightsExecutor, RecordingFieldAccessRightsExecutor>();
            });
        }
    }
}