using FubuMVC.Core;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests
{
    
    public class FubuApplication_registers_the_FubuRuntime_in_bootstrap
    {
        [Fact]
        public void runtime_is_in_the_container_and_does_not_cause_a_stackoverflow_when_it_disposes()
        {
            using (var runtime = FubuRuntime.Basic())
            {
                runtime.Get<FubuRuntime>()
                    .ShouldBeTheSameAs(runtime);
            }
        }
    }
}