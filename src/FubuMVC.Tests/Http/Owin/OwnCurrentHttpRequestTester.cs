using System.Collections.Generic;
using FubuMVC.Core.Http.Owin;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Http.Owin
{
    
    public class OwinCurrentHttpRequest_FullUrl_Tester
    {
        private IDictionary<string, object> environment;
        private IDictionary<string, string[]> headers;
        private OwinHttpRequest request;

        public OwinCurrentHttpRequest_FullUrl_Tester()
        {
            environment = new Dictionary<string, object>();
            headers = new Dictionary<string, string[]>();
            environment.Add(OwinConstants.RequestHeadersKey, headers);
            environment.Add(OwinConstants.RequestSchemeKey, "https");
            
            
            setHeader("Host", "localhost");
            request = new OwinHttpRequest(environment);

        }

        private void setHeader(string header, string value)
        {
            headers[header] = new []{value};
        }

        [Fact]
        public void should_prepend_scheme()
        {
            request.FullUrl().ShouldStartWith("https://");
        }

        [Fact]
        public void should_support_host_without_port()
        {
            request.FullUrl().ShouldStartWith("https://localhost");
        }

        [Fact]
        public void should_support_host_with_port()
        {
            setHeader("Host", "localhost:8080");
            request.FullUrl().ShouldStartWith("https://localhost:8080");
        }

        [Fact]
        public void should_support_ip_address_with_port()
        {
            setHeader("Host", "127.0.0.1:8080");
            request.FullUrl().ShouldStartWith("https://127.0.0.1:8080");
        }

        [Fact]
        public void should_be_tolerant_of_invalid_host_format()
        {
            setHeader("Host", "localhost:  ");
            request.FullUrl().ShouldStartWith("https://localhost");
        }

        [Fact]
        public void should_ignore_invalid_port()
        {
            setHeader("Host", "localhost:a");
            request.FullUrl().ShouldStartWith("https://localhost");
        }

        [Fact]
        public void should_use_path()
        {
            environment[OwinConstants.RequestPathKey] = "/foo";
            request.FullUrl().ShouldBe("https://localhost/foo");
        }

        [Fact]
        public void should_support_querystring()
        {
            environment[OwinConstants.RequestQueryStringKey] = "baz=foo";
            request.FullUrl().ShouldBe("https://localhost/?baz=foo");
        }

        [Fact]
        public void should_support_path_and_querystring()
        {
            environment[OwinConstants.RequestPathKey] = "/foo";
            environment[OwinConstants.RequestQueryStringKey] = "baz=foo";
            request.FullUrl().ShouldBe("https://localhost/foo?baz=foo");
        }
    }
}