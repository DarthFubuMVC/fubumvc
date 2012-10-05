using System;
using System.Net;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Endpoints;
using FubuMVC.Core.Runtime;
using HtmlTags;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Core.UI.Testing.Integration
{

    [TestFixture]
    public class doughnut_caching_tester : SharedHarnessContext
    {
        [Test]
        public void doughnut_caching()
        {
            var text1 = endpoints.Get<CachedEndpoints>(x => x.get_doughnut_cached()).ReadAsText();
            var text2 = endpoints.Get<CachedEndpoints>(x => x.get_doughnut_cached()).ReadAsText();
            var text3 = endpoints.Get<CachedEndpoints>(x => x.get_doughnut_cached()).ReadAsText();
            var text4 = endpoints.Get<CachedEndpoints>(x => x.get_doughnut_cached()).ReadAsText();

            text1.ShouldEqual(text2);
            text1.ShouldEqual(text3);
            text1.ShouldEqual(text4);
        }
    }

    public class CachedEndpoints
    {
        private readonly FubuHtmlDocument _document;
        private readonly IOutputWriter _writer;

        public CachedEndpoints(FubuHtmlDocument document, IOutputWriter writer)
        {
            _document = document;
            _writer = writer;
        }



        public HtmlDocument get_doughnut_cached()
        {
            _writer.AppendHeader(HttpResponseHeader.ETag, Guid.NewGuid().ToString());

            _document.Add(new LiteralTag(_document.Partial<DateRequest>().ToString()));

            return _document;
        }

        [Cache]
        public HtmlTag get_cached_Name(CachedRequest request)
        {
            return new HtmlTag("h1").Text(Guid.NewGuid().ToString());
        }

        [Cache]
        public HtmlTag DatePartial(DateRequest request)
        {
            return new HtmlTag("h1").Text(Guid.NewGuid().ToString());
        }
    }

    public class CachedRequest
    {
        public string Name { get; set; }
    }

    public class DateRequest{}

    public class SharedHarnessContext
    {
        protected EndpointDriver endpoints
        {
            get
            {
                UrlContext.Stub(SelfHostHarness.Root);

                return SelfHostHarness.Endpoints;
            }
        }
    }
}