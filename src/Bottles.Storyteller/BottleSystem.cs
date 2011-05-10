using System;
using StoryTeller.Engine;

namespace Bottles.Storyteller
{
    public class BottleSystem : ISystem
    {
        public object Get(Type type)
        {
            throw new NotImplementedException();
        }

        public void RegisterServices(ITestContext context)
        {
        }

        public void SetupEnvironment()
        {
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