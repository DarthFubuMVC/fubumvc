using System;
using System.Reflection;
using FubuMVC.Core.Diagnostics.Querying;
using StoryTeller.Engine;

namespace StoryTellerTesting
{
    public class FubuSystem : ISystem
    {
        public static readonly string TEST_APPLICATION_ROOT = "http://localhost/fubu-testing";
        private CommandRunner _runner;
        private ApplicationDriver _application;

        public object Get(Type type)
        {
            throw new NotImplementedException();
        }

        public void RegisterServices(ITestContext context)
        {
            var remoteGraph = new RemoteBehaviorGraph(TEST_APPLICATION_ROOT);
            context.Store(remoteGraph);

            context.Store(_runner);

            context.Store(_application);
        }

        public void SetupEnvironment()
        {
            // TODO -- make this configurable?
            _runner = new CommandRunner();
            _runner.RunFubu("createvdir src/FubuTestApplication fubu-testing");

            _application = new ApplicationDriver();
        }

        public void TeardownEnvironment()
        {
            _application.Teardown();
        }

        public void Setup()
        {
        }

        public void Teardown()
        {
        }

        public void RegisterFixtures(FixtureRegistry registry)
        {
            registry.AddFixturesFromThisAssembly();
        }
    }
}