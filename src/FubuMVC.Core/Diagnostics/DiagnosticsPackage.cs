using System;
using FubuCore.Binding;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security;

namespace FubuMVC.Core.Diagnostics
{
    public class DiagnosticsPackage : IFubuRegistryExtension
    {
        public void Configure(FubuRegistry registry)
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

    public static class DiagnosticsExtensions
    {
        public static bool IsDiagnosticsEnabled(this BehaviorGraph graph)
        {
            //TODO: need a better way to determine if diagnostics are enabled
            var outputWriter = graph.Services.DefaultServiceFor<IOutputWriter>();
            if (outputWriter != null && outputWriter.Type == typeof(DebuggingOutputWriter))
            {
                return true;
            }
            return false;
        }
    }
}