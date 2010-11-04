using System;
using System.Linq;
using FubuMVC.Core.Diagnostics.Querying;
using StoryTeller;
using StoryTeller.Engine;

namespace StoryTellerTesting.Fixtures
{
    public class PackagingFixture : Fixture
    {
        private readonly RemoteBehaviorGraph _remoteGraph;
        private readonly CommandRunner _runner;

        public PackagingFixture(RemoteBehaviorGraph remoteGraph, CommandRunner runner)
        {
            _remoteGraph = remoteGraph;
            _runner = runner;


            this["SetUp"] = Embed<PackagingSetupFixture>("If the packaging setup is");        
        }

        [FormatAs("No endpoints registered for assembly {assemblyName}")]
        public bool NoActionsForAssembly(string assemblyName)
        {
            return !_remoteGraph.All().EndpointsForAssembly(assemblyName).Any();
        }

        
    }

    public class Package1Fixture : Fixture
    {
        private readonly RemoteBehaviorGraph _remoteGraph;

        public Package1Fixture(RemoteBehaviorGraph remoteGraph)
        {
            _remoteGraph = remoteGraph;

            Title = "For TestPackage1 (Assembly 'TestPackage1')";
        }

        public IGrammar RoutesShouldBe()
        {
            return VerifySetOf(() => _remoteGraph.All().EndpointsForAssembly("TestPackage1"))
                .Titled("The routes and actions from TestPackage1 should be")
                .MatchOn(x => x.RoutePattern, x => x.FirstActionDescription);
        }
    }

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