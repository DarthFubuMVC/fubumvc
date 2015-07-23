using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Owin;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests.Http.Owin
{
    [TestFixture]
    public class OwinHttpResponseTester
    {
        [Test]
        public void do_not_blow_up_when_there_are_no_headers()
        {
            new OwinHttpResponse().AllHeaders().Any().ShouldBeFalse();
        }

        [Test]
        public void write_a_header_that_allows_multiple_values()
        {
            var settings = new OwinHeaderSettings();
            var environment = new Dictionary<string, object>
            {
                {OwinConstants.HeaderSettings, settings}
            };
            var response = new OwinHttpResponse(environment);

            response.AppendHeader(HttpGeneralHeaders.Allow, "application/json");
            response.AppendHeader(HttpGeneralHeaders.Allow, "text/json");

            var headers = environment.Get<IDictionary<string, string[]>>(OwinConstants.ResponseHeadersKey);
            headers[HttpGeneralHeaders.Allow].ShouldHaveTheSameElementsAs("application/json", "text/json");
        }

        [Test]
        public void write_a_header_that_does_not_allow_multiple_values()
        {
            var settings = new OwinHeaderSettings();
            var environment = new Dictionary<string, object>
            {
                {OwinConstants.HeaderSettings, settings}
            };
            var response = new OwinHttpResponse(environment);

            settings.DoNotAllowMultipleValues(HttpRequestHeaders.ContentLength);

            response.AppendHeader(HttpRequestHeaders.ContentLength, "1234");
            response.AppendHeader(HttpRequestHeaders.ContentLength, "1234");

            var headers = environment.Get<IDictionary<string, string[]>>(OwinConstants.ResponseHeadersKey);
            headers[HttpRequestHeaders.ContentLength].ShouldHaveTheSameElementsAs("1234");
        }
    }
}
