using System;
using System.IO;
using System.Net;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Assets.Http;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Assets.Http
{
    [TestFixture]
    public class when_executing_the_cache_writer : InteractionContext<AssetCacheWriter>
    {
        private DateTime _theDate;
        private AssetFile theFile;

        protected override void beforeEach()
        {
            theFile = new AssetFile("herpderp") {FullPath = Path.GetRandomFileName()};
            Services.PartialMockTheClassUnderTest();
            _theDate = new DateTime(2011, 1, 1);
            ClassUnderTest.LastModifiedLambda = (path) => _theDate;
            FubuMode.Reset();
        }

        [Test]
        public void should_write_cache_headers_when_not_in_development()
        {
            ClassUnderTest.Write(theFile);

            ClassUnderTest.AssertWasCalled(x => x.WriteCachingHeaders(theFile));
        }

        [Test]
        public void should_not_write_cache_headers_in_development()
        {
            FubuMode.Mode(FubuMode.Development);

            ClassUnderTest.Write(theFile);

            ClassUnderTest.AssertWasNotCalled(x => x.WriteCachingHeaders(null),ctx => ctx.IgnoreArguments());
        }
    }

    [TestFixture]
    public class when_writing_the_cache_headers : InteractionContext<AssetCacheWriter>
    {
        private DateTime _theDate;
        private AssetFile theFile;

        protected override void beforeEach()
        {
            theFile = new AssetFile("herpderp") {FullPath = Path.GetRandomFileName()};
            Services.PartialMockTheClassUnderTest();
            _theDate = new DateTime(2011, 1, 1);
            ClassUnderTest.LastModifiedLambda = (path) => _theDate;
            FubuMode.Reset();
            ClassUnderTest.WriteCachingHeaders(theFile);
        }

        [Test]
        public void should_write_the_last_modified_header()
        {
            MockFor<IOutputWriter>()
                .AssertWasCalled(x => x.AppendHeader(HttpResponseHeader.LastModified, _theDate.ToString("R")));
        }

        [Test]
        public void should_write_max_age_to_24_hours()
        {
            MockFor<IOutputWriter>()
                .AssertWasCalled(x => x.AppendHeader(HttpResponseHeader.CacheControl, "private, max-age={0}".ToFormat(24*60*60)));
        }

    }

}