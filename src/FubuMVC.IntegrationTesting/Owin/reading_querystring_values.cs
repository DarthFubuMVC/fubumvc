﻿using FubuCore;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration.Nodes;
using Shouldly;
using Xunit;

namespace FubuMVC.IntegrationTesting.Owin
{
    
    public class reading_querystring_values
    {
        [Fact]
        public void read_querystring_values_from_current_request()
        {
            TestHost.BehaviorGraph.ChainFor<ReadingQuerystringEndpoint>(x => x.get_querystring_Key(null))
                .As<RoutedChain>()
                .GetRoutePattern().ShouldBe("querystring/{Key}");

            TestHost.Scenario(_ =>
            {
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