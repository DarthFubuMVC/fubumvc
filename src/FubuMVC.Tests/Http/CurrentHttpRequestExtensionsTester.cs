using FubuMVC.Core.Http;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Http
{
    [TestFixture]
    public class CurrentHttpRequestExtensionsTester
    {
        private StandInCurrentHttpRequest theRequest;

        [SetUp]
        public void SetUp()
        {
            theRequest = new StandInCurrentHttpRequest();
        }

        [Test]
        public void is_get()
        {
            theRequest.TheHttpMethod = "get";
            theRequest.IsGet().ShouldBeTrue();

            theRequest.TheHttpMethod = "GET";
            theRequest.IsGet().ShouldBeTrue();

            theRequest.TheHttpMethod = "POST";
            theRequest.IsGet().ShouldBeFalse();
        }

        [Test]
        public void is_post()
        {
            theRequest.TheHttpMethod = "post";
            theRequest.IsPost().ShouldBeTrue();

            theRequest.TheHttpMethod = "POST";
            theRequest.IsPost().ShouldBeTrue();

            theRequest.TheHttpMethod = "get";
            theRequest.IsPost().ShouldBeFalse();

        }

        [Test]
        public void is_put()
        {
            theRequest.TheHttpMethod = "put";
            theRequest.IsPut().ShouldBeTrue();

            theRequest.TheHttpMethod = "PUT";
            theRequest.IsPut().ShouldBeTrue();

            theRequest.TheHttpMethod = "get";
            theRequest.IsPut().ShouldBeFalse();
        }

        [Test]
        public void relative_url()
        {
            theRequest.TheRelativeUrl = "";
            theRequest.ToRelativeContentUrl("/foo")
                .ShouldEqual("foo");

            theRequest.TheRelativeUrl = "/bar";
            theRequest.ToRelativeContentUrl("/foo")
                .ShouldEqual("../foo");

            theRequest.TheRelativeUrl = "/bar";
            theRequest.ToRelativeContentUrl("/bar/1")
                .ShouldEqual("1");


        }
    }
}