using System.Diagnostics;
using FubuMVC.Core;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting
{
    [TestFixture]
    public class performance_timing_smoke_tester
    {
        [Test]
        public void see_what_it_looks_like()
        {
            using (var runtime = FubuRuntime.Basic())
            {
                Debug.WriteLine(runtime.ActivationDiagnostics.Timer.DisplayTimings());
            }
        }
    }
}