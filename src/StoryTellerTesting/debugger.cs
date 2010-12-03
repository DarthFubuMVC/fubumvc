using IntegrationTesting;
using NUnit.Framework;

namespace StoryTellerTesting
{
    [TestFixture]
    public class debugger
    {
        [Test]
        public void try_to_set_up_the_environment()
        {
            var system = new FubuSystem();
            system.SetupEnvironment();
        }
    }
}