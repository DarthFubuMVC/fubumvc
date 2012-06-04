using FubuMVC.Core;
using FubuMVC.Diagnostics.Runtime;
using FubuMVC.Diagnostics.Runtime.Tracing;

namespace FubuMVC.Diagnostics
{
    public class DiagnosticsRegistration : IFubuRegistryExtension
    {
        public void Configure(FubuRegistry registry)
        {
            registry.Policies.Add<ApplyTracing>();
            registry.Import<AdvancedDiagnosticsRegistry>(DiagnosticUrlPolicy.DIAGNOSTICS_URL_ROOT);

        }
    }
}