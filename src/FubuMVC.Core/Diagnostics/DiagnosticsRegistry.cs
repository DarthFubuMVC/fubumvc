using System.Collections.Generic;
using FubuCore.Binding;
using FubuCore.Reflection;
using FubuMVC.Core.Diagnostics.HtmlWriting;
using FubuMVC.Core.Diagnostics.Querying;
using FubuMVC.Core.Diagnostics.Tracing;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security;

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
                .IncludeType<ScriptWriter>();

            Routes
                .UrlPolicy<DiagnosticUrlPolicy>();


            Services(x =>
            {
                x.SetServiceIfNone<IBindingLogger, RecordingBindingLogger>();
                x.SetServiceIfNone<IDebugDetector, DebugDetector>();
                x.SetServiceIfNone<IDebugCallHandler, DebugCallHandler>();
                x.ReplaceService<IObjectResolver, RecordingObjectResolver>();
                x.ReplaceService<IDebugReport, DebugReport>();
                x.ReplaceService<IRequestObserver, RequestObserver>();
                x.ReplaceService<IRequestData, RecordingRequestData>();
                x.ReplaceService<IFubuRequest, RecordingFubuRequest>();
                x.ReplaceService<IDebugDetector, DebugDetector>();
                x.ReplaceService<IAuthorizationPolicyExecutor, RecordingAuthorizationPolicyExecutor>();
                x.ReplaceService<IOutputWriter, RecordingOutputWriter>();
            });
        }
    }
}