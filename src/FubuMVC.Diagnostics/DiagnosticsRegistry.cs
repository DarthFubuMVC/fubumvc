using FubuCore.Binding.InMemory;
using FubuCore.Logging;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Diagnostics.Model;
using FubuMVC.Diagnostics.Runtime;
using FubuMVC.Diagnostics.Runtime.Tracing;
using FubuMVC.Diagnostics.Visualization;

namespace FubuMVC.Diagnostics
{
    public class DiagnosticsRegistry : FubuPackageRegistry
    {
        // default policies are good enough
    }

    public class DiagnosticServiceRegistry : ServiceRegistry
    {
        public DiagnosticServiceRegistry()
        {
            SetServiceIfNone<IDebugDetector, DebugDetector>();
            ReplaceService<IDebugDetector, DebugDetector>();
            
            // does no harm
            ReplaceService<IBindingHistory, BindingHistory>();
            SetServiceIfNone<IRequestHistoryCache, RequestHistoryCache>();

            AddService<IRequestTraceObserver, RequestHistoryObserver>();
            

            SetServiceIfNone<IRequestTrace, RequestTrace>();
            SetServiceIfNone<IRequestLogBuilder, RequestLogBuilder>();

            SetServiceIfNone<IVisualizer, Visualizer>();
            SetServiceIfNone<IDiagnosticContext, DiagnosticContext>();
        }
    }

    public class VerboseServiceRegistry : ServiceRegistry
    {
        public VerboseServiceRegistry()
        {
        }
    }
}