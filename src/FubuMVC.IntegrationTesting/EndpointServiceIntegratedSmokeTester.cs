using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Runtime;
using Shouldly;
using Xunit;

namespace FubuMVC.IntegrationTesting
{
    
    public class EndpointServiceIntegratedSmokeTester
    {
        [Fact]
        public void chain_is_on_the_endpoint()
        {
            var runtime = FubuRuntime.For<SomeRegistry>();
            var endpoint = runtime.Get<IEndpointService>().EndpointFor<SomeEndpoint>(x => x.get_hello());

            endpoint.Chain.Calls.Single().HandlerType.ShouldBe(typeof (SomeEndpoint));
        }
    }

    public class SomeRegistry : FubuRegistry
    {
    }

    public class SomeEndpoint
    {
        public string get_hello()
        {
            return "Hello.";
        }
    }
}