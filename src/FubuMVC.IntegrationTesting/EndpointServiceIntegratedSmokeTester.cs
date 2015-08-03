using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Runtime;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting
{
    [TestFixture]
    public class EndpointServiceIntegratedSmokeTester
    {
        [Test]
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