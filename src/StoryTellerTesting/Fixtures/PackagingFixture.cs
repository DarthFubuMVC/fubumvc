using System.Linq;
using FubuMVC.Core.Diagnostics.Querying;
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

        [FormatAs("Some endpoints are registered for assembly {assemblyName}")]
        public bool ActionsForAssemblyArePresent(string assemblyName)
        {
            return _remoteGraph.All().EndpointsForAssembly(assemblyName).Any();
        }
    }
}