using FubuMVC.Core;
using NUnit.Framework;
using FubuMVC.StructureMap;
using StructureMap;
using System.Linq;
using FubuTestingSupport;

namespace FubuMVC.IntegrationTesting
{
    [TestFixture]
    public class EndpointServiceIntegratedSmokeTester
    {
        [Test]
        public void chain_is_on_the_endpoint()
        {
            var runtime = FubuApplication.For<SomeRegistry>().StructureMap(new Container()).Bootstrap();
            var endpoint = runtime.Factory.Get<IEndpointService>().EndpointFor<SomeEndpoint>(x => x.get_hello());

            endpoint.Chain.Calls.Single().HandlerType.ShouldEqual(typeof (SomeEndpoint));
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