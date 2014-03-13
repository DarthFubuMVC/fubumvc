using System;
using System.Collections.Generic;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Owin;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Http
{
    [TestFixture]
    public class CurrentHttpRequestExtensionsTester
    {
        private OwinHttpRequest theRequest;

        [SetUp]
        public void SetUp()
        {
            theRequest = new OwinHttpRequest();
        }

        [Test]
        public void is_get()
        {
            theRequest.HttpMethod("get");
            theRequest.IsGet().ShouldBeTrue();

            theRequest.HttpMethod("GET");
            theRequest.IsGet().ShouldBeTrue();

            theRequest.HttpMethod("POST");
            theRequest.IsGet().ShouldBeFalse();
        }

        [Test]
        public void is_head()
        {
            theRequest.HttpMethod("head");
            theRequest.IsHead().ShouldBeTrue();

            theRequest.HttpMethod("HEAD");
            theRequest.IsHead().ShouldBeTrue();

            theRequest.HttpMethod("POST");
            theRequest.IsHead().ShouldBeFalse();
        }

        [Test]
        public void is_post()
        {
            theRequest.HttpMethod("post");
            theRequest.IsPost().ShouldBeTrue();

            theRequest.HttpMethod("POST");
            theRequest.IsPost().ShouldBeTrue();

            theRequest.HttpMethod("get");
            theRequest.IsPost().ShouldBeFalse();

        }

        [Test]
        public void is_put()
        {
            theRequest.HttpMethod("put");
            theRequest.IsPut().ShouldBeTrue();

            theRequest.HttpMethod("PUT");
            theRequest.IsPut().ShouldBeTrue();

            theRequest.HttpMethod("get");
            theRequest.IsPut().ShouldBeFalse();
        }

        [Test]
        public void relative_url()
        {
            theRequest.RelativeUrl("");
            theRequest.ToRelativeContentUrl("/foo")
                .ShouldEqual("foo");

            theRequest.RelativeUrl("/bar");
            theRequest.ToRelativeContentUrl("/foo")
                .ShouldEqual("../foo");

            theRequest.RelativeUrl("/bar");
            theRequest.ToRelativeContentUrl("/bar/1")
                .ShouldEqual("1");


        }

        [Test]
        public void get_comma_separated_values_from_header()
        {
            new[] { "v1", "v2, v3", "\"v4, b\"", "v5, v6", "v7", }
                .GetCommaSeparatedHeaderValues()
                .ShouldHaveTheSameElementsAs("v1", "v2", "v3", "v4, b", "v5", "v6", "v7");

            new []{"v1,v2, v3,\"v4, b\",v5, v6,v7"}
                .GetCommaSeparatedHeaderValues()
                .ShouldHaveTheSameElementsAs("v1", "v2", "v3", "v4, b", "v5", "v6", "v7");
        }


        [Test]
        public void etag_matches_with_no_values()
        {
            new string[0].EtagMatches("foo")
                .ShouldEqual(EtagMatch.None);
        }

        [Test]
        public void etag_matches_with_wildcard()
        {
            new string[] {"a", "*", "b"}
                .EtagMatches("foo")
                .ShouldEqual(EtagMatch.Yes);
        }

        [Test]
        public void etag_matches_positive()
        {
            new string[] {"a", "b", "foo"}
                .EtagMatches("foo")
                .ShouldEqual(EtagMatch.Yes);
        }

        [Test]
        public void etag_matches_negative()
        {
            new string[] { "a", "b", "bar" }
                .EtagMatches("foo")
                .ShouldEqual(EtagMatch.No);
        }
    }
}