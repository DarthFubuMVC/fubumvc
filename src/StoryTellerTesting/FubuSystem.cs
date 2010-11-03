using System;
using FubuMVC.Core.Diagnostics.Querying;
using StoryTeller.Engine;

namespace StoryTellerTesting
{
    public class FubuSystem : ISystem
    {
        public static readonly string TEST_APPLICATION_ROOT = "http://localhost/fubu-testing";

        public object Get(Type type)
        {
            throw new NotImplementedException();
        }

        public void RegisterServices(ITestContext context)
        {
            var remoteGraph = new RemoteBehaviorGraph(TEST_APPLICATION_ROOT);
            context.Store(remoteGraph);
        }

        public void SetupEnvironment()
        {
            // Need to setup the virtual dir here?
        }

        public void TeardownEnvironment()
        {
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