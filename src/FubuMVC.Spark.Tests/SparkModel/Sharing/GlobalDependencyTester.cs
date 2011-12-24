using FubuMVC.Spark.SparkModel.Sharing;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Spark.Tests.SparkModel.Sharing
{
    [TestFixture]
    public class GlobalDependencyTester : InteractionContext<SharingGraph.GlobalDependency>
    {
        private const string Dependency = "GlobalX";
        protected override void beforeEach()
        {
            ClassUnderTest.Dependency = Dependency;
        }

        [Test]
        public void equality_checks()
        {
            ClassUnderTest.ShouldEqual(new SharingGraph.GlobalDependency { Dependency = Dependency });
            ClassUnderTest.ShouldNotEqual(new SharingGraph.GlobalDependency { Dependency = "GlobalY" });
        }
    }
}