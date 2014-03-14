using System.Linq;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.View.Model.Sharing;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.View.Model.Sharing
{
    [TestFixture]
    public class SharingGraphTester : InteractionContext<SharingGraph>
    {
        protected override void beforeEach()
        {
            ClassUnderTest.Global("global");

            ClassUnderTest.Dependency("a", "b");
            ClassUnderTest.Dependency("a", "c");
            ClassUnderTest.Dependency("c", "b");

        }

        [Test]
        public void application_is_always_applicable()
        {
            ClassUnderTest.SharingsFor("a").Last().ShouldEqual("Application");
            ClassUnderTest.SharingsFor("b").Last().ShouldEqual("Application");
            ClassUnderTest.SharingsFor("c").Last().ShouldEqual("Application");
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
            ClassUnderTest.SharingsFor("a").ShouldHaveTheSameElementsAs("b", "c", "global", ContentFolder.Application);
        }

        [Test]
        public void dependencies_are_set_correctly_2()
        {
            ClassUnderTest.SharingsFor("c").ShouldHaveTheSameElementsAs("b", "global", ContentFolder.Application);
        }

        [Test]
        public void when_dependent_is_dependency_it_is_not_registered()
        {
            ClassUnderTest.Dependency("x", "x");



            ClassUnderTest.SharingsFor("x").ShouldNotContain("x");
        }
    }
}