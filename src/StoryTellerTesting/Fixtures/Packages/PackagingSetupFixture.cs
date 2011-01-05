using StoryTeller.Engine;
using FubuCore;

namespace IntegrationTesting.Fixtures.Packages
{
    [Hidden]
    public class PackagingSetupFixture : Fixture
    {
        private readonly CommandRunner _runner;

        public PackagingSetupFixture(CommandRunner runner)
        {
            _runner = runner;
        }

        public override void SetUp(ITestContext context)
        {
            _runner.RunFubu("packages fubu-testing -removeall");
        }

        [FormatAs("Install package {zipFile}")]
        public void InstallPackage(string zipFile)
        {
            _runner.RunFubu("install-pak {0} fubu-testing".ToFormat(zipFile));            
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