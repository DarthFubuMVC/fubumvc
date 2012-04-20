using System;
using System.Collections.Generic;
using System.Net;
using FubuCore;
using FubuCore.Binding;
using FubuCore.Binding.Values;
using FubuMVC.Core;
using FubuMVC.Core.Assets.Http;
using FubuMVC.Core.Http;
using FubuMVC.Core.Resources.Etags;
using NUnit.Framework;
using FubuTestingSupport;
using Rhino.Mocks;

namespace FubuMVC.Tests.Assets.Http
{
    public class StubHeaders : IRequestHeaders
    {
        public KeyValues Data = new KeyValues();

        public void Value<T>(string header, Action<T> callback)
        {
            Data.ForValue(header, (key, value) => callback(value.As<T>()));
        }

        public T BindToHeaders<T>()
        {
            throw new NotImplementedException();
        }

        public bool HasHeader(string header)
        {
            return Data.Has(header);
        }

        public bool IsAjaxRequest()
        {
            throw new NotImplementedException();
        }
    }

    [TestFixture]
    public class AssetEtagInvocationFilterTester
    {
        private ServiceArguments theServiceArguments;
        private EtagCache theCache;
        private AssetEtagInvocationFilter theFilter;
        private KeyValues theHeaders;

        private void stash<T>() where T : class
        {
            theServiceArguments.Set(typeof(T), MockRepository.GenerateMock<T>());
        }

        private void setRequestIfNoneMatch(string etag)
        {

            theHeaders[HttpRequestHeaders.IfNoneMatch] = etag;
        }

        [SetUp]
        public void SetUp()
        {
            theHeaders = new KeyValues();
            var requestData = new RequestData(new FlatValueSource(theHeaders, RequestDataSource.Header.ToString()));

            theServiceArguments = new ServiceArguments();

            theServiceArguments.Set(typeof(IRequestData), requestData);

            stash<IHttpWriter>();
            stash<ICurrentChain>();

            theCache = new EtagCache();

            theFilter = new AssetEtagInvocationFilter(theCache);
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
            var theResourceHash = Guid.NewGuid().ToString();
            theServiceArguments.Get<ICurrentChain>().Stub(x => x.ResourceHash())
                .Return(theResourceHash);

            setRequestIfNoneMatch("12345");

            theCache.Register(theResourceHash, "12345");

            theFilter.Filter(theServiceArguments).ShouldEqual(DoNext.Stop);

            theServiceArguments.Get<IHttpWriter>()
                .AssertWasCalled(x => x.WriteResponseCode(HttpStatusCode.NotModified));
        }

        [Test]
        public void should_return_continue_if_the_etag_does_not_match_the_current_version()
        {
            var theResourceHash = Guid.NewGuid().ToString();
            theServiceArguments.Get<ICurrentChain>().Stub(x => x.ResourceHash())
                .Return(theResourceHash);

            setRequestIfNoneMatch("12345");

            theCache.Register(theResourceHash, "12345-6");

            theFilter.Filter(theServiceArguments).ShouldEqual(DoNext.Continue);

            theServiceArguments.Get<IHttpWriter>()
                .AssertWasNotCalled(x => x.WriteResponseCode(HttpStatusCode.NotModified));
        }
    }

    
}