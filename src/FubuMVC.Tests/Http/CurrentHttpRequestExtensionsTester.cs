using System;
using System.Collections.Generic;
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
        public void is_head()
        {
            theRequest.TheHttpMethod = "head";
            theRequest.IsHead().ShouldBeTrue();

            theRequest.TheHttpMethod = "HEAD";
            theRequest.IsHead().ShouldBeTrue();

            theRequest.TheHttpMethod = "POST";
            theRequest.IsHead().ShouldBeFalse();
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