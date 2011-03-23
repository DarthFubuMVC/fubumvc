using FubuCore.Binding;
using FubuMVC.Core.Diagnostics.Querying;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security;
using FubuMVC.Core.UI.Diagnostics;
using FubuCore.Reflection;

namespace FubuMVC.Core.Diagnostics
{
    public class DiagnosticsRegistry : FubuRegistry
    {
        public DiagnosticsRegistry()
        {
            Applies.ToAssemblyContainingType<DiagnosticsRegistry>();

            Actions.IncludeTypes(x => x.HasAttribute<FubuDiagnosticsAttribute>()).IncludeType<GraphQuery>().IncludeType<ScriptWriter>();
            Routes.UrlPolicy<DiagnosticUrlPolicy>();

            Services(x =>
            {
                x.ReplaceService<IOutputWriter, DebuggingOutputWriter>();
                x.ReplaceService<IObjectResolver, RecordingObjectResolver>();
                x.ReplaceService<IDebugReport, DebugReport>();
                x.ReplaceService<IRequestData, RecordingRequestData>();
                x.ReplaceService<IFubuRequest, RecordingFubuRequest>();
                x.ReplaceService<IDebugDetector, DebugDetector>();
                x.ReplaceService<IAuthorizationPolicyExecutor, RecordingAuthorizationPolicyExecutor>();
            });
        }
    }
}