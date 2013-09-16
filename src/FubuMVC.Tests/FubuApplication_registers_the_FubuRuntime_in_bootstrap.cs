using FubuMVC.Core;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests
{
    [TestFixture]
    public class FubuApplication_registers_the_FubuRuntime_in_bootstrap
    {
        [Test]
        public void runtime_is_in_the_container_and_does_not_cause_a_stackoverflow_when_it_disposes()
        {
            using (var runtime = FubuApplication.DefaultPolicies().StructureMap().Bootstrap())
            {
                runtime.Factory.Get<FubuRuntime>()
                    .ShouldBeTheSameAs(runtime);
            }
        }
    }
}