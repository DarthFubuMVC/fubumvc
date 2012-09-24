using FubuMVC.Core.Endpoints;

namespace FubuMVC.IntegrationTesting
{
    public class SharedHarnessContext
    {
        protected EndpointDriver endpoints
        {
            get
            {
                return SelfHostHarness.Endpoints;
            }
        }
    }
}