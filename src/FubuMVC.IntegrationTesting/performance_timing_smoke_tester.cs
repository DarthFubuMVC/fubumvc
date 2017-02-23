using System.Diagnostics;
using FubuMVC.Core;
using Xunit;

namespace FubuMVC.IntegrationTesting
{
    
    public class performance_timing_smoke_tester
    {
        [Fact]
        public void see_what_it_looks_like()
        {
            using (var runtime = FubuRuntime.Basic())
            {
                Debug.WriteLine(runtime.ActivationDiagnostics.Timer.DisplayTimings());
            }
        }
    }
}