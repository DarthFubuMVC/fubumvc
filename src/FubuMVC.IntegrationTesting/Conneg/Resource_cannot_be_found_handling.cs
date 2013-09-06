using System.Net;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Conneg
{
    [TestFixture]
    public class Resource_cannot_be_found_handling : SharedHarnessContext
    {
        [Test]
        public void the_resource_not_found_handler_should_kick_in()
        {
            var response = endpoints.Get<MissingResourceEndpoint>(x => x.get_missing());
            response.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }
    }

    public class MissingResourceEndpoint
    {
        public MissingResource get_missing()
        {
            return null;
        }
    }

    public class MissingResource{}
}