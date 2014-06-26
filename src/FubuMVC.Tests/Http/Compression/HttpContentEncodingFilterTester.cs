using FubuCore.Binding;
using FubuCore.Binding.Values;
using FubuMVC.Core;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Compression;
using FubuMVC.Core.Http.Owin;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Http.Compression
{
    [TestFixture]
    public class HttpContentEncodingFilterTester : InteractionContext<HttpContentEncodingFilter>
    {
        private IHttpContentEncoding theEncoding;
        private string theAcceptedEncoding;
        private KeyValues theHeaders;
        private ServiceArguments theArguments;

        protected override void beforeEach()
        {
            theArguments = new ServiceArguments();
            theEncoding = MockFor<IHttpContentEncoding>();
            theAcceptedEncoding = "gzip, deflate, sdch";

            theEncoding.Stub(x => x.MatchingEncoding).Return(ContentEncoding.GZip);
            MockFor<IHttpContentEncoders>().Stub(x => x.MatchFor(theAcceptedEncoding)).Return(theEncoding);

            var request = new OwinHttpRequest();
            request.Header(HttpRequestHeaders.AcceptEncoding, theAcceptedEncoding);



            theArguments.Set(typeof(IHttpRequest), request);
            theArguments.Set(typeof(IHttpResponse), MockFor<IHttpResponse>());

            ClassUnderTest.Filter(theArguments).ShouldEqual(DoNext.Continue);
        }

        [Test]
        public void sets_the_encoding_in_the_http_writer()
        {
            MockFor<IHttpResponse>().AssertWasCalled(x => x.UseEncoding(theEncoding));
        }

        [Test]
        public void sets_the_content_encoding_header()
        {
            MockFor<IHttpResponse>().AssertWasCalled(x => x.AppendHeader(HttpResponseHeaders.ContentEncoding, "gzip"));
        }
    }
}