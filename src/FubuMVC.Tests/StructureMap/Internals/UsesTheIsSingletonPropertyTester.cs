using FubuCore.Dates;
using FubuMVC.Core;
using FubuTestingSupport;
using NUnit.Framework;
using StructureMap;
using StructureMap.Pipeline;

namespace FubuMVC.Tests.StructureMap.Internals
{
    [TestFixture]
    public class UsesTheIsSingletonPropertyTester
    {
        [Test]
        public void IClock_should_be_a_singleton_just_by_usage_of_the_IsSingleton_property()
        {
            using (var runtime = FubuApplication.For(new FubuRegistry()).Bootstrap())
            {
                runtime.Factory.Get<IContainer>().Model.For<IClock>().Default.Lifecycle.ShouldBeOfType<SingletonLifecycle>();
            }
        }
    }
}