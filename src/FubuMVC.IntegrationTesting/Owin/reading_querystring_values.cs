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
            TestHost.BehaviorGraph.BehaviorFor<ReadingQuerystringEndpoint>(x => x.get_querystring_Key(null))
                    .As<RoutedChain>()
                    .GetRoutePattern().ShouldEqual("querystring/{Key}");

            TestHost.Scenario(_ => {
                _.Get.Url("/querystring/Foo?Foo=Bar&A=1&B=2");
                _.ContentShouldBe("Bar");
            });

            TestHost.Scenario(_ =>
            {
                _.Get.Url("/querystring/A?Foo=Bar&A=1&B=2");
                _.ContentShouldBe("1");
            });

            TestHost.Scenario(_ =>
            {
                _.Get.Url("/querystring/B?Foo=Bar&A=1&B=2");
                _.ContentShouldBe("2");
            });


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