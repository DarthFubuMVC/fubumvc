using System;
using FubuCore;
using FubuMVC.Core.Diagnostics.Querying;
using StoryTeller.Engine;

namespace IntegrationTesting
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

            _runner.RunFubu("init-pak src/TestPackage1 pak1");
            _runner.RunFubu("init-pak src/TestPackage2 pak2");
            _runner.RunFubu("init-pak src/TestPackage3 pak3");

            _runner.RunFubu("create-pak pak1 pak1.zip -force");
            _runner.RunFubu("create-pak pak2 pak2.zip -force");
            _runner.RunFubu("create-pak pak3 pak3.zip -force");

            _application = new ApplicationDriver();
        }

        public void TeardownEnvironment()
        {
            _application.Teardown();

            var fileSystem = new FileSystem();
            fileSystem.DeleteFile("pak1.zip");
            fileSystem.DeleteFile("pak2.zip");
            fileSystem.DeleteFile("pak3.zip");
        }

        public void Setup()
        {
        }

        public void Teardown()
        {
            _application.Teardown();
        }

        public void RegisterFixtures(FixtureRegistry registry)
        {
            registry.AddFixturesFromThisAssembly();
        }
    }
}