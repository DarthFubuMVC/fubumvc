using System.Diagnostics;
using FubuMVC.Core;
using FubuMVC.Core.StructureMap;
using FubuMVC.StructureMap;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting
{
    [TestFixture]
    public class performance_timing_smoke_tester
    {
        [Test]
        public void see_what_it_looks_like()
        {
            using (var runtime = FubuApplication.DefaultPolicies().Bootstrap())
            {
                Debug.WriteLine(runtime.Behaviors.Diagnostics.Timer.DisplayTimings());
            }

        }
    }
}