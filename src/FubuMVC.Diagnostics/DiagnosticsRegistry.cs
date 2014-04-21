using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Diagnostics.Model;
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
            SetServiceIfNone<IVisualizer, Visualizer>();
            SetServiceIfNone<IDiagnosticContext, DiagnosticContext>();
        }
    }
}