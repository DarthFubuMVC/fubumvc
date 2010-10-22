using FubuCore.Binding;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security;

namespace FubuMVC.Core.Diagnostics
{
    public class DiagnosticsPackage : IRegistryModification
    {
        public void Modify(FubuRegistry registry)
        {
            registry.Services(x =>
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