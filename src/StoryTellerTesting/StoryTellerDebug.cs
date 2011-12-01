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
            runner = new ProjectTestRunner(@"C:\code\fubumvc\Storyteller.xml");
            runner.Project.TimeoutInSeconds = 0;
        }

        [Test]
        public void Invoke_a_Json_endpoint_from_a_package()
        {
            runner.RunAndAssertTest("Packaging/Zip File Mechanics/Invoke a Json endpoint from a package from Zip");
        }

        [TestFixtureTearDown]
        public void TeardownRunner()
        {
            runner.Dispose();
        }
    }
}