using FubuMVC.Core;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration;
using FubuMVC.Katana;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.OwinHost.Testing
{
    [TestFixture]
    public class reading_querystring_values
    {
        [Test]
        public void read_querystring_values_from_current_request()
        {
            using (var server = FubuApplication.DefaultPolicies().StructureMap().RunEmbedded())
            {
                server.Services.GetInstance<BehaviorGraph>()
                    .BehaviorFor<ReadingQuerystringEndpoint>(x => x.get_querystring_Key(null))
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
        private readonly ICurrentHttpRequest _request;

        public ReadingQuerystringEndpoint(ICurrentHttpRequest request)
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