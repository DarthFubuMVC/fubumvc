using System;
using System.Net;
using FubuMVC.Core;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.UI;
using FubuMVC.Katana;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;
using StructureMap;

namespace FubuMVC.IntegrationTesting.UI
{

    [TestFixture]
    public class doughnut_caching_tester
    {
        [Test]
        public void doughnut_caching()
        {
            using (var server = FubuApplication.DefaultPolicies().StructureMap(new Container()).RunEmbedded())
            {
                var endpoints = server.Endpoints;

                var text1 = endpoints.Get<CachedEndpoints>(x => x.get_doughnut_cached()).ReadAsText();
                var text2 = endpoints.Get<CachedEndpoints>(x => x.get_doughnut_cached()).ReadAsText();
                var text3 = endpoints.Get<CachedEndpoints>(x => x.get_doughnut_cached()).ReadAsText();
                var text4 = endpoints.Get<CachedEndpoints>(x => x.get_doughnut_cached()).ReadAsText();

                text1.ShouldEqual(text2);
                text1.ShouldEqual(text3);
                text1.ShouldEqual(text4);
            }
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

    public class DateRequest{}


}