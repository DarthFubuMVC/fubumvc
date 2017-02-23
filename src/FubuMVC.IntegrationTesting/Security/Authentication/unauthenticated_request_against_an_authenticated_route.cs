﻿using System.Net;
using FubuMVC.Core.Http;
using FubuMVC.Core.Security.Authentication;
using Xunit;

namespace FubuMVC.IntegrationTesting.Security.Authentication
{
    
    public class unauthenticated_request_against_an_authenticated_route : AuthenticationHarness
    {
        [Fact]
        public void redirects_to_login()
        {
            var loginUrl = Urls.UrlFor(new LoginRequest {Url = "some/authenticated/route"}, "GET");

            Scenario(_ =>
            {
                _.Get.Input(new TargetModel());
                _.Request.Accepts("text/html");

                _.StatusCodeShouldBe(HttpStatusCode.Redirect);
                _.Header(HttpResponseHeaders.Location).SingleValueShouldEqual(loginUrl);
            });
        }
    }
}