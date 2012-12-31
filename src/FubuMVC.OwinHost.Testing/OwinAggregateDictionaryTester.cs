using System.Collections.Generic;
using System.Web.Routing;
using FubuMVC.Core.Http;
using FubuMVC.OwinHost.Readers;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.OwinHost.Testing
{
    [TestFixture]
    public class OwinAggregateDictionaryTester
    {
        [Test]
        public void should_have_request_locator()
        {
            var environment = new Dictionary<string, object>();
            var headers = new Dictionary<string, string[]>();
            headers.Add(OwinConstants.ContentTypeHeader, new []{"text/xml"});
            environment[OwinConstants.RequestHeadersKey] = headers;
            environment[OwinConstants.RequestQueryStringKey] = "";
            new OwinRequestReader().Read(environment);
            var dictionary = new OwinRequestData(new RouteData(), environment, new StandInCurrentHttpRequest());
            dictionary.ValuesFor(OwinRequestData.Querystring).ShouldNotBeNull();
            dictionary.ValuesFor(OwinRequestData.FormPost).ShouldNotBeNull();
        }
    }
}