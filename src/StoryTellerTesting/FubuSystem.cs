using System;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics.Querying;
using FubuTestApplication;
using Serenity;
using StoryTeller.Engine;
using FubuMVC.StructureMap;
using StructureMap;

namespace IntegrationTesting
{
    public class FubuTestApplicationSource : IApplicationSource
    {
        public FubuApplication BuildApplication()
        {
            return FubuApplication.For<FubuTestApplicationRegistry>().StructureMap(new Container());
        }

        public string Name
        {
            get { return "FubuTestApplication"; }
        }
    }

    public class FubuSystem : SerenitySystem
    {
        public static readonly string TEST_APPLICATION_ROOT = "http://localhost/fubu-testing";
        private CommandRunner _runner;
        private ApplicationDriver _application;

        public FubuSystem()
        {
            AddApplication<FubuTestApplicationSource>();
        }

        public override object Get(Type type)
        {
            return null;
        }

        public override void RegisterServices(ITestContext context)
        {
            base.RegisterServices(context);
            var remoteGraph = new RemoteBehaviorGraph(TEST_APPLICATION_ROOT);
            context.Store(remoteGraph);

            context.Store(_runner);

            context.Store(_application);
        }

        public override void SetupEnvironment()
        {
            base.SetupEnvironment();

            // TODO -- make this configurable?
            _runner = new CommandRunner();
            _runner.RunFubu("alias fubu-testing src/FubuTestApplication");
            _runner.RunFubu("createvdir src/FubuTestApplication fubu-testing");

            _runner.RunFubu("create-pak pak1 pak1.zip -f");
            _runner.RunFubu("create-pak pak2 pak2.zip -f");
            _runner.RunFubu("create-pak pak3 pak3.zip -f");
			_runner.RunFubu("create-pak pak4 pak4.zip -f");
			_runner.RunFubu("create-pak spark spark.zip -f");

            _application = new ApplicationDriver();
        }

        public override void TeardownEnvironment()
        {
            base.TeardownEnvironment();

            _application.Teardown();

            var fileSystem = new FileSystem();
            fileSystem.DeleteFile("pak1.zip");
            fileSystem.DeleteFile("pak2.zip");
            fileSystem.DeleteFile("pak3.zip");
			fileSystem.DeleteFile("pak4.zip");
			fileSystem.DeleteFile("spark.zip");
        }

        public override void Setup()
        {
        }

        public override void Teardown()
        {
            _application.Teardown();
        }

        public override void RegisterFixtures(FixtureRegistry registry)
        {
            registry.AddFixturesFromThisAssembly();
        }
    }
}