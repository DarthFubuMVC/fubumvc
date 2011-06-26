using FubuCore.Binding;
using FubuMVC.Core.Diagnostics.HtmlWriting;
using FubuMVC.Core.Diagnostics.Querying;
using FubuMVC.Core.Diagnostics.Tracing;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security;
using FubuCore.Reflection;

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
                             x.ReplaceService<IObjectResolver, RecordingObjectResolver>();
                             x.ReplaceService<IDebugReport, DebugReport>();
                             x.ReplaceService<IRequestData, RecordingRequestData>();
                             x.ReplaceService<IFubuRequest, RecordingFubuRequest>();
                             x.ReplaceService<IDebugDetector, DebugDetector>();
                             x.ReplaceService<IAuthorizationPolicyExecutor, RecordingAuthorizationPolicyExecutor>();
                             x.ReplaceService<IOutputWriter, RecordingOutputWriter>();
                             x.ReplaceService<IModelBinderCache, RecordingModelBinderCache>();
                             x.ReplaceService<IPropertyBinderCache, RecordingPropertyBinderCache>();
                             x.ReplaceService<IValueConverterRegistry, RecordingValueConverterRegistry>();
                         });
        }
    }
}