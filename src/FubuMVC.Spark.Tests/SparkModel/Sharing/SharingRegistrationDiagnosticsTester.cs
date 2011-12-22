using System.Linq;
using FubuMVC.Spark.SparkModel.Sharing;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Spark.Tests.SparkModel.Sharing
{
    [TestFixture]
    public class SharingRegistrationDiagnosticsTester : InteractionContext<SharingRegistrationDiagnostics>
    {
        private SharingLogsCache _logs;

        protected override void beforeEach()
        {
            Container.Inject(_logs = new SharingLogsCache());

            ClassUnderTest.SetCurrentProvenance("x");
            ClassUnderTest.Global("global");
            ClassUnderTest.Dependency("a", "b");
        }

        [Test]
        public void global_invokes_inner()
        {
            MockFor<ISharingRegistration>()
                .AssertWasCalled(x => x.Global("global"));
        }

        [Test]
        public void global_adds_to_named_log()
        {
            var entry = first_entry_by_name("global");
            entry.Provenance.ShouldEqual("x");
            entry.Message.ShouldContain("acts as global");
        }        

        [Test]
        public void dependency_invokes_inner()
        {
            MockFor<ISharingRegistration>()
                .AssertWasCalled(x => x.Dependency("a", "b"));            
        }

        [Test]
        public void depdendency_adds_dependent_to_named_log()
        {
            var entry = first_entry_by_name("a");
            entry.Provenance.ShouldEqual("x");
            entry.Message.ShouldContain("requires b");
        }

        [Test]
        public void depdendency_adds_dependency_to_named_log()
        {
            var entry = first_entry_by_name("b");
            entry.Provenance.ShouldEqual("x");
            entry.Message.ShouldContain("is required by a");            
        }

        [Test]
        public void provenance_is_set_correctly()
        {
            ClassUnderTest.SetCurrentProvenance("p");
            ClassUnderTest.CurrentProvenance.ShouldEqual("p");
        }

        public SharingLogEntry first_entry_by_name(string name)
        {
            return _logs.FindByName(name).Logs.First();
        }
    }
}