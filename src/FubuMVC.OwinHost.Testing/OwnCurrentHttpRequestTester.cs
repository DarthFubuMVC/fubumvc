using System.Collections.Generic;
using Gate;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.OwinHost.Testing
{
    [TestFixture]
    public class OwnCurrentHttpRequest_FullUrl_Tester
    {
        private Environment environment;
        private OwinRequestBody body;
        private OwinCurrentHttpRequest request;

        [SetUp]
        public void SetUp()
        {
            environment = new Environment();
            environment.Headers = new Dictionary<string, string>();
            environment.Scheme = "https";
            environment.Headers["Host"] = "localhost";
            body = new OwinRequestBody(environment);
            request = new OwinCurrentHttpRequest(body);
        }

        [Test]
        public void should_prepend_scheme()
        {
            request.FullUrl().ShouldStartWith("https://");
        }

        [Test]
        public void should_support_host_without_port()
        {
            request.FullUrl().ShouldStartWith("https://localhost");
        }

        [Test]
        public void should_support_host_with_port()
        {
            environment.Headers["Host"] = "localhost:8080";
            request.FullUrl().ShouldStartWith("https://localhost:8080");
        }

        [Test]
        public void should_support_ip_address_with_port()
        {
            environment.Headers["Host"] = "127.0.0.1:8080";
            request.FullUrl().ShouldStartWith("https://127.0.0.1:8080");
        }

        [Test]
        public void should_be_tolerant_of_invalid_host_format()
        {
            environment.Headers["Host"] = "localhost:  ";
            request.FullUrl().ShouldStartWith("https://localhost");
        }

        [Test]
        public void should_ignore_invalid_port()
        {
            environment.Headers["Host"] = "localhost:a";
            request.FullUrl().ShouldStartWith("https://localhost");
        }

        [Test]
        public void should_use_path()
        {
            environment.Path = "/foo";
            request.FullUrl().ShouldEqual("https://localhost/foo");
        }

        [Test]
        public void should_support_querystring()
        {
            environment.QueryString = "baz=foo";
            request.FullUrl().ShouldEqual("https://localhost/?baz=foo");
        }

        [Test]
        public void should_support_path_and_querystring()
        {
            environment.Path = "/foo";
            environment.QueryString = "baz=foo";
            request.FullUrl().ShouldEqual("https://localhost/foo?baz=foo");
        }
    }
}