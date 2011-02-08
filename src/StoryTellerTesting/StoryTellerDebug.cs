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
            runner = new ProjectTestRunner(@"C:\git\fubumvc\Storyteller.xml");
        }

        [Test]
        public void Invoke_a_Json_endpoint_from_a_package()
        {
            runner.RunAndAssertTest("FastPack/Grids/Query options for a grid with strings and numbers");
        }

        [TestFixtureTearDown]
        public void TeardownRunner()
        {
            runner.Dispose();
        }
    }
}