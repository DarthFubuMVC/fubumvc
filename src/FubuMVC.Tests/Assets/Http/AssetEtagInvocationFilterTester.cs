using System;
using System.Collections.Generic;
using System.Net;
using FubuCore.Binding;
using FubuMVC.Core;
using FubuMVC.Core.Assets.Http;
using FubuMVC.Core.Http;
using FubuMVC.Core.Resources.Etags;
using NUnit.Framework;
using FubuTestingSupport;
using Rhino.Mocks;

namespace FubuMVC.Tests.Assets.Http
{
    [TestFixture]
    public class AssetEtagInvocationFilterTester
    {
        private ServiceArguments theServiceArguments;
        private AggregateDictionary theDictionary;
        private Dictionary<string, object> theHeaders;
        private EtagCache theCache;
        private AssetEtagInvocationFilter theFilter;

        private void stash<T>() where T : class
        {
            theServiceArguments.Set(typeof(T), MockRepository.GenerateMock<T>());
        }

        private void setRequestIfNoneMatch(string etag)
        {
            theHeaders.Add(HttpRequestHeaders.IfNoneMatch, etag);
        }

        [SetUp]
        public void SetUp()
        {
            theServiceArguments = new ServiceArguments();
            theDictionary = new AggregateDictionary();
            theHeaders = new Dictionary<string, object>();
            theDictionary.AddDictionary(RequestDataSource.Header.ToString(), theHeaders);

            theServiceArguments.Set(typeof(AggregateDictionary), theDictionary);

            stash<IHttpWriter>();
            stash<ICurrentChain>();

            theCache = new EtagCache();

            theFilter = new AssetEtagInvocationFilter(theCache);
        }

        [Test]
        public void returns_continue_if_there_is_no_if_none_match_header()
        {
            theHeaders.ContainsKey(HttpRequestHeaders.IfNoneMatch).ShouldBeFalse();

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