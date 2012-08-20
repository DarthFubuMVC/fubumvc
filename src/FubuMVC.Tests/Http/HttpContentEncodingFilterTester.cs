using FubuCore.Binding;
using FubuMVC.Core;
using FubuMVC.Core.Http;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Http
{
    [TestFixture]
    public class HttpContentEncodingFilterTester : InteractionContext<HttpContentEncodingFilter>
    {
        private IHttpContentEncoding theEncoding;
        private string theAcceptedEncoding;
        private StubRequestHeaders theRequestHeaders;
        private ServiceArguments theArguments;

        protected override void beforeEach()
        {
            theEncoding = MockFor<IHttpContentEncoding>();
            theAcceptedEncoding = "gzip, deflate, sdch";

            MockFor<IHttpContentEncoders>().Stub(x => x.MatchFor(theAcceptedEncoding)).Return(theEncoding);
            
            theRequestHeaders = new StubRequestHeaders();
            theRequestHeaders.Data[HttpRequestHeaders.AcceptEncoding] = theAcceptedEncoding;
            Services.Inject<IRequestHeaders>(theRequestHeaders);

            theArguments = new ServiceArguments();
            theArguments.Set(typeof(IRequestHeaders), theRequestHeaders);
            theArguments.Set(typeof(IHttpContentEncoders), MockFor<IHttpContentEncoders>());
            theArguments.Set(typeof(IHttpWriter), MockFor<IHttpWriter>());

            ClassUnderTest.Filter(theArguments).ShouldEqual(DoNext.Continue);
        }

        [Test]
        public void sets_the_encoding_in_the_http_writer()
        {
            MockFor<IHttpWriter>().AssertWasCalled(x => x.UseEncoding(theEncoding));
        }
    }
}