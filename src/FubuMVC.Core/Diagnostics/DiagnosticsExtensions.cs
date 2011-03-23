using FubuMVC.Core.Diagnostics.Tracing;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Diagnostics
{
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