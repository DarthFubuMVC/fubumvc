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
        public void Create_a_Zip_file_for_a_Package()
        {
            runner.RunAndAssertTest("Packaging/Zip File Mechanics/Create a Zip file for a Package");
        }

        [TestFixtureTearDown]
        public void TeardownRunner()
        {
            runner.Dispose();
        }
    }
}