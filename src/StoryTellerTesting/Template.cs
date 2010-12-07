using NUnit.Framework;
using StoryTeller.Execution;

namespace IntegrationTesting
{
    [TestFixture, Explicit]
    public class Template
    {
        private ProjectTestRunner runner;

        [TestFixtureSetUp]
        public void SetupRunner()
        {
            runner = new ProjectTestRunner(@"C:\git\fubumvc\src\StoryTellerTesting\Storyteller.xml");
        }

        [Test]
        public void Load_actions_from_a_package_in_dev_link_mode()
        {
            runner.RunAndAssertTest("Packaging/Load actions from a package in dev link mode");
        }

        [TestFixtureTearDown]
        public void TeardownRunner()
        {
            runner.Dispose();
        }
    }
}