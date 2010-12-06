using NUnit.Framework;

namespace IntegrationTesting
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