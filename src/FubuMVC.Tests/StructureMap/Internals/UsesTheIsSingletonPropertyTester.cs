using FubuCore.Dates;
using FubuMVC.Core;
using Shouldly;
using Xunit;
using StructureMap;
using StructureMap.Pipeline;

namespace FubuMVC.Tests.StructureMap.Internals
{
    
    public class UsesTheIsSingletonPropertyTester
    {
        [Fact]
        public void IClock_should_be_a_singleton_just_by_usage_of_the_IsSingleton_property()
        {
            using (var runtime = FubuRuntime.Basic())
            {
                runtime.Get<IContainer>().Model.For<IClock>().Default.Lifecycle.ShouldBeOfType<SingletonLifecycle>();
            }
        }
    }
}