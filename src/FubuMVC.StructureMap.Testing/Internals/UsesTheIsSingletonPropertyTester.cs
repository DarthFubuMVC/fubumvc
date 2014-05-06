using FubuCore.Dates;
using FubuMVC.Core;
using FubuTestingSupport;
using NUnit.Framework;
using StructureMap;
using StructureMap.Pipeline;

namespace FubuMVC.StructureMap.Testing.Internals
{
    [TestFixture]
    public class UsesTheIsSingletonPropertyTester
    {
        [Test]
        public void IClock_should_be_a_singleton_just_by_usage_of_the_IsSingleton_property()
        {
            var container = new Container();
            FubuApplication.For(new FubuRegistry()).StructureMap(container).Bootstrap();

            container.Model.For<IClock>().Lifecycle.ShouldBeOfType<SingletonLifecycle>();
        }
    }
}