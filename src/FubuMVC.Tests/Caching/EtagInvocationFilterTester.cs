using System;
using System.Net;
using FubuCore.Binding;
using FubuCore.Binding.Values;
using FubuMVC.Core;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Headers;
using FubuMVC.Core.Resources.Etags;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Caching
{
    [TestFixture]
    public class EtagInvocationFilterTester
    {
        private ServiceArguments theServiceArguments;
        private HeadersCache theCache;
        private EtagInvocationFilter theFilter;
        private KeyValues theHeaders;
        private string theHash;
        private Func<ServiceArguments, string> theSource;

        private void stash<T>() where T : class
        {
            theServiceArguments.Set(typeof(T), MockRepository.GenerateMock<T>());
        }

        private void setRequestIfNoneMatch(string etag)
        {
            var request = theServiceArguments.Get<IHttpRequest>();
            request
                               .Stub(x => x.HasHeader(HttpRequestHeader.IfNoneMatch))
                               .Return(true);


            request.Stub(x => x.GetHeader(HttpRequestHeader.IfNoneMatch)).Return(new string[] {etag});


            theHeaders[HttpRequestHeaders.IfNoneMatch] = etag;
        }

        [SetUp]
        public void SetUp()
        {
            theHeaders = new KeyValues();
            var requestData = new RequestData(new FlatValueSource(theHeaders, RequestDataSource.Header.ToString()));

            theServiceArguments = new ServiceArguments();

            theServiceArguments.Set(typeof(IRequestData), requestData);

            stash<IHttpResponse>();
            stash<ICurrentChain>();
            stash<IHttpRequest>();

            theCache = new HeadersCache();

            theHash = Guid.NewGuid().ToString();

            theSource = MockRepository.GenerateMock<Func<ServiceArguments, string>>();
            theSource.Stub(x => x.Invoke(theServiceArguments)).Return(theHash);

            theFilter = new EtagInvocationFilter(theCache, theSource);
        }

        [Test]
        public void returns_continue_if_there_is_no_if_none_match_header()
        {
            theHeaders.Has(HttpRequestHeaders.IfNoneMatch).ShouldBeFalse();

            theFilter.Filter(theServiceArguments).ShouldEqual(DoNext.Continue);
        }

        [Test]
        public void should_return_stop_and_write_304_response_code_if_the_ifnonematch_header_matches_the_current_etag()
        {
            setRequestIfNoneMatch("12345");

            theCache.Register(theHash, new Header[] { new Header(HttpResponseHeader.ETag, "12345"), });

            theFilter.Filter(theServiceArguments).ShouldEqual(DoNext.Stop);

            theServiceArguments.Get<IHttpResponse>()
                .AssertWasCalled(x => x.WriteResponseCode(HttpStatusCode.NotModified));
        }

        [Test]
        public void if_the_etag_matches_the_resource_write_additional_headers_too()
        {
            setRequestIfNoneMatch("12345");

            theCache.Register(theHash, new Header[] { new Header(HttpResponseHeader.ETag, "12345"), new Header("a", "1"), new Header("b", "2"), new Header("c", "3")});

            theFilter.Filter(theServiceArguments).ShouldEqual(DoNext.Stop);

            var writer = theServiceArguments.Get<IHttpResponse>();

            writer.AssertWasCalled(x => x.AppendHeader("a", "1"));
            writer.AssertWasCalled(x => x.AppendHeader("b", "2"));
            writer.AssertWasCalled(x => x.AppendHeader("c", "3"));
        }

        [Test]
        public void should_return_continue_if_the_etag_does_not_match_the_current_version()
        {
            setRequestIfNoneMatch("12345");

            theCache.Register(theHash, new Header[] { new Header(HttpResponseHeader.ETag, "12345-6"), });

            theFilter.Filter(theServiceArguments).ShouldEqual(DoNext.Continue);

            theServiceArguments.Get<IHttpResponse>()
                .AssertWasNotCalled(x => x.WriteResponseCode(HttpStatusCode.NotModified));
        }
    }
}