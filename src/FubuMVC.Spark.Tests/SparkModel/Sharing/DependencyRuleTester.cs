using FubuMVC.Spark.SparkModel.Sharing;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Spark.Tests.SparkModel.Sharing
{
    [TestFixture]
    public class DependencyRuleTester : InteractionContext<SharingGraph.DependencyRule>
    {
        private const string Dependency = "Dependency";
        private const string Dependent = "Dependent";

        protected override void beforeEach()
        {
            ClassUnderTest.Dependency = Dependency;
            ClassUnderTest.Dependent = Dependent;
        }

        [Test]
        public void equality_checks()
        {
            ClassUnderTest.ShouldEqual(new SharingGraph.DependencyRule { Dependency = Dependency, Dependent = Dependent });
            ClassUnderTest.ShouldNotEqual(new SharingGraph.DependencyRule { Dependency = Dependency });
            ClassUnderTest.ShouldNotEqual(new SharingGraph.DependencyRule { Dependent = Dependent });
            ClassUnderTest.ShouldNotEqual(new SharingGraph.DependencyRule { Dependency = "Foo", Dependent = "Bar" });
        }
    }
}