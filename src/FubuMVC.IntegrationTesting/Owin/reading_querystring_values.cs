using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Katana;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Owin
{
    [TestFixture]
    public class reading_querystring_values
    {
        [Test]
        public void read_querystring_values_from_current_request()
        {
            using (var server = FubuApplication.DefaultPolicies().StructureMap().RunEmbeddedWithAutoPort())
            {
                server.Services.GetInstance<BehaviorGraph>()
                    .BehaviorFor<ReadingQuerystringEndpoint>(x => x.get_querystring_Key(null))
                    .As<RoutedChain>()
                    .GetRoutePattern().ShouldEqual("querystring/{Key}");

                server.Endpoints.Get("querystring/Foo?Foo=Bar&A=1&B=2")
                    .ReadAsText().ShouldEqual("Bar");

                server.Endpoints.Get("querystring/A?Foo=Bar&A=1&B=2")
                    .ReadAsText().ShouldEqual("1");

                server.Endpoints.Get("querystring/B?Foo=Bar&A=1&B=2")
                    .ReadAsText().ShouldEqual("2");
            }


        }
    }

    public class ReadingQuerystringEndpoint
    {
        private readonly IHttpRequest _request;

        public ReadingQuerystringEndpoint(IHttpRequest request)
        {
            _request = request;
        }

        public string get_querystring_Key(QuerystringRequest request)
        {
            return _request.QueryString[request.Key];
        }
    }

    public class QuerystringRequest
    {
        public string Key { get; set; }
    }
}