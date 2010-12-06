using NUnit.Framework;
using StoryTeller.Execution;

namespace StoryTellerTestHarness
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
        public void Invoke_a_Json_endpoint_from_a_package()
        {
            runner.RunAndAssertTest("Packaging/Invoke a Json endpoint from a package");
        }

        [TestFixtureTearDown]
        public void TeardownRunner()
        {
            runner.Dispose();
        }
    }
}