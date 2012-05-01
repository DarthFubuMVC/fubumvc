using System;

using FubuMVC.Core.Diagnostics.HtmlWriting;
using FubuMVC.Core.Diagnostics.HtmlWriting.Columns;
using FubuMVC.Core.Diagnostics.Tracing;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;

namespace FubuMVC.Tests.Diagnostics
{
    using FubuMVC.Core;

    public class StubCurrentHttpRequest : ICurrentHttpRequest
    {
        private readonly string _baseUrl;

        public StubCurrentHttpRequest(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public string RawUrl()
        {
            throw new NotImplementedException();
        }

        public string RelativeUrl()
        {
            throw new NotImplementedException();
        }

        public string FullUrl()
        {
            throw new NotImplementedException();
        }

        public string ToFullUrl(string url)
        {
            return url.ToAbsoluteUrl(_baseUrl);
        }

        public string HttpMethod()
        {
            throw new NotImplementedException();
        }
    }

    [TestFixture]
    public class RouteColumnTester
    {
        [Test]
        public void write_route_column_when_the_route_does_not_exist()
        {
            var chain = new BehaviorChain();

            var tag = new HtmlTag("td");

            new RouteColumn(new StubCurrentHttpRequest("http://server")).WriteBody(chain, null, tag);

            tag.Text().ShouldEqual(" -");
        }

        [Test]
        public void write_route_column_when_the_route_exists()
        {
            var chain = new BehaviorChain();
            chain.Route = new RouteDefinition("some/thing/else");

            var row = new TableRowTag();
            var tag = row.Cell();

            new RouteColumn(new StubCurrentHttpRequest("http://server")).WriteBody(chain, null, tag);

            tag.FirstChild().Text().ShouldEqual(chain.Route.Pattern);
            row.HasClass(BehaviorGraphWriter.FUBU_INTERNAL_CLASS).ShouldBeFalse();
        }

        [Test]
        public void write_route_column_for_internal_diagnostics_route()
        {
            var chain = new BehaviorChain();
            chain.Route = new RouteDefinition(DiagnosticUrlPolicy.DIAGNOSTICS_URL_ROOT + "/chains");

            var row = new TableRowTag();
            var tag = row.Cell();

            new RouteColumn(new StubCurrentHttpRequest("http://server")).WriteBody(chain, row, tag);

            row.HasClass(BehaviorGraphWriter.FUBU_INTERNAL_CLASS).ShouldBeTrue();
        }

    }
}