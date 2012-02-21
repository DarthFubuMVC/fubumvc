using System.Web.Routing;
using FubuMVC.Core.Http;
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
            var dictionary = new OwinAggregateDictionary(new RouteData(), new OwinRequestBody(new Environment()));
            dictionary.DataFor(RequestDataSource.Request.ToString()).ShouldNotBeNull();
        }
    }
}