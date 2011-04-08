using FubuCore.Binding;
using FubuMVC.Core.Diagnostics.Tracing;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core.Diagnostics
{
    public static class DiagnosticsExtensions
    {
        public static bool IsDiagnosticsEnabled(this BehaviorGraph graph)
        {
            //TODO: need a better way to determine if diagnostics are enabled
			var requestData = graph.Services.DefaultServiceFor<IRequestData>();
			if (requestData != null && requestData.Type == typeof(RecordingRequestData))
            {
                return true;
            }
            return false;
        }
    }
}