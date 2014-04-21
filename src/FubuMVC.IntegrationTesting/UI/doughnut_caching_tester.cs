using System;
using System.Net;
using FubuMVC.Core;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.UI;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.UI
{
    [TestFixture]
    public class doughnut_caching_tester
    {
        [SetUp]
        public void SetUp()
        {
            FubuMode.Reset();
        }

        [Test]
        public void doughnut_caching()
        {
            var text1 = TestHost.GetByAction<CachedEndpoints>(x => x.get_doughnut_cached()).Body.ReadAsText();
            var text2 = TestHost.GetByAction<CachedEndpoints>(x => x.get_doughnut_cached()).Body.ReadAsText();
            var text3 = TestHost.GetByAction<CachedEndpoints>(x => x.get_doughnut_cached()).Body.ReadAsText();
            var text4 = TestHost.GetByAction<CachedEndpoints>(x => x.get_doughnut_cached()).Body.ReadAsText();

            text1.ShouldEqual(text2);
            text1.ShouldEqual(text3);
            text1.ShouldEqual(text4);
        }
    }

    public class CachedEndpoints
    {
        private readonly FubuHtmlDocument _document;
        private readonly IOutputWriter _writer;
        private readonly IPartialInvoker _partials;

        public CachedEndpoints(FubuHtmlDocument document, IOutputWriter writer, IPartialInvoker partials)
        {
            _document = document;
            _writer = writer;
            _partials = partials;
        }


        public HtmlDocument get_doughnut_cached()
        {
            _writer.AppendHeader(HttpResponseHeader.ETag, Guid.NewGuid().ToString());

            var html = _partials.Invoke<DateRequest>();

            _document.Add(new LiteralTag(html));

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

    public class DateRequest
    {
    }
}