using FubuMVC.Spark.SparkModel.Sharing;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Spark.Tests.SparkModel.Sharing
{
    public class SharingGraphTester : InteractionContext<SharingGraph>
    {
        protected override void beforeEach()
        {
            ClassUnderTest.Global("global");

            ClassUnderTest.Dependency("a", "b");
            ClassUnderTest.Dependency("a", "c");
            ClassUnderTest.Dependency("c", "b");

            ClassUnderTest.CompileDependencies("a", "b", "c", "global");
        }

        [Test]
        public void when_compiling_dependencies_globals_are_registered_as_dependencies()
        {
            ClassUnderTest.SharingsFor("a").ShouldContain("global");
            ClassUnderTest.SharingsFor("b").ShouldContain("global");
            ClassUnderTest.SharingsFor("c").ShouldContain("global");
        }

        [Test]
        public void dependencies_are_set_correctly_1()
        {
            ClassUnderTest.SharingsFor("a").ShouldHaveTheSameElementsAs("b", "c", "global");
        }

        [Test]
        public void dependencies_are_set_correctly_2()
        {
            ClassUnderTest.SharingsFor("c").ShouldHaveTheSameElementsAs("b", "global");
        }

        [Test]
        public void when_dependent_is_dependency_it_is_not_registered()
        {
            ClassUnderTest.Dependency("x", "x");
            ClassUnderTest.SharingsFor("x").ShouldHaveCount(0);
        }
    }
}