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
        public void Query_options_for_a_grid_with_strings_and_numbers()
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