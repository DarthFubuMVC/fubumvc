using Bottles.Diagnostics;
using FubuMVC.Spark.SparkModel.Sharing;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Spark.Tests.SparkModel.Sharing
{
    [TestFixture]
    public class RecordingSharingRegistrationTester : InteractionContext<RecordingSharingRegistration>
    {
        protected override void beforeEach()
        {
            ClassUnderTest.Global("g");
            ClassUnderTest.Dependency("a", "b");

            ClassUnderTest.Apply(MockFor<IPackageLog>(), MockFor<ISharingRegistration>());
        }

        [Test]
        public void it_records_global()
        {
            MockFor<ISharingRegistration>().AssertWasCalled(x => x.Global("g"));
        }

        [Test]
        public void it_records_dependency()
        {
            MockFor<ISharingRegistration>().AssertWasCalled(x => x.Dependency("a", "b"));
        }
    }
}