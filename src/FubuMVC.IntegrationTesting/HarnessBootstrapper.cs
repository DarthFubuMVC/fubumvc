using NUnit.Framework;

namespace FubuMVC.IntegrationTesting
{
    [SetUpFixture]
    public class HarnessBootstrapper
    {
        [TearDown]
        public void TearDown()
        {
            TestHost.Shutdown();
        }
    }
}