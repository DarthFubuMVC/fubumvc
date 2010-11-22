using StoryTeller.Engine;

namespace StoryTellerTesting.Fixtures
{
    [Hidden]
    public class PackagingSetupFixture : Fixture
    {
        private readonly CommandRunner _runner;

        public PackagingSetupFixture(CommandRunner runner)
        {
            _runner = runner;
        }

        [FormatAs("No packages are included")]
        public void NoPackages()
        {
            _runner.RunFubu("link fubu-testing -cleanall");
        }

        [FormatAs("From the command line, run:  {commandLine}")]
        public void Run(string commandLine)
        {
            _runner.RunFubu(commandLine);
        }

        public override void TearDown()
        {
            _runner.RunFubu("restart fubu-testing");
        }
    }
}