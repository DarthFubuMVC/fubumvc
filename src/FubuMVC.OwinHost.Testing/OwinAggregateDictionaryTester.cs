using System.Web.Routing;
using FubuTestingSupport;
using Gate;
using NUnit.Framework;

namespace FubuMVC.OwinHost.Testing
{
    [TestFixture]
    public class OwinAggregateDictionaryTester
    {
        [Test]
        public void should_have_request_locator()
        {
            var dictionary = new OwinRequestData(new RouteData(), new OwinRequestBody(new Environment()));
            dictionary.ValuesFor(OwinRequestData.Querystring).ShouldNotBeNull();
            dictionary.ValuesFor(OwinRequestData.FormPost).ShouldNotBeNull();
        }
    }
}