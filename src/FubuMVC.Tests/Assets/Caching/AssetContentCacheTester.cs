using System;
using FubuMVC.Core;
using FubuMVC.Core.Assets.Caching;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Resources.Etags;
using NUnit.Framework;
using FubuTestingSupport;
using Rhino.Mocks;

namespace FubuMVC.Tests.Assets.Caching
{
    [TestFixture]
    public class AssetContentCacheTester
    {
        private readonly string resource1 = Guid.NewGuid().ToString();
        private readonly string resource2 = Guid.NewGuid().ToString();
        private readonly string resource3 = Guid.NewGuid().ToString();
        private readonly string resource4 = Guid.NewGuid().ToString();

        private AssetFile file1;
        private AssetFile file2;
        private AssetFile file3;
        private AssetFile file4;
        private AssetFile file5;
        private AssetFile file6;
        private AssetContentCache theCache;
        private BehaviorGraph theGraph;
        private IHeadersCache headersCache;
        private IOutputCache theOutputCache;

        private IRecordedOutput getOutputWithEtag(string etag)
        {
            var output = new RecordedOutput(null);
            output.AppendHeader(HttpResponseHeaders.ETag, etag);

            return output;
        }

        private IRecordedOutput storeAgainstResource(string resourceHash)
        {
            var output = getOutputWithEtag(Guid.NewGuid().ToString());
            theOutputCache.Retrieve(resourceHash, () => output);

            return output;
        }

        [SetUp]
        public void SetUp()
        {
            file1 = new AssetFile("1");
            file2 = new AssetFile("2");
            file3 = new AssetFile("3");
            file4 = new AssetFile("4");
            file5 = new AssetFile("5");
            file6 = new AssetFile("6");

            theOutputCache = MockRepository.GenerateMock<IOutputCache>();
            headersCache = MockRepository.GenerateMock<IHeadersCache>();

            theCache = new AssetContentCache(headersCache, theOutputCache);

            theGraph = BehaviorGraph.BuildFrom(new FubuRegistry());
        }



        [Test]
        public void flush_all_removes_the_content_and_ejects_all_of_the_header_cache()
        {
            var hash1 = "12345";
            var hash2 = "23456";
            var hash3 = "34567";

            theCache.LinkFilesToResource(hash1, new AssetFile[] { file1, file2, file3 });
            theCache.LinkFilesToResource(hash2, new AssetFile[] { file2, file3, file4 });
            theCache.LinkFilesToResource(hash3, new AssetFile[] { file5 });
        
            theCache.FlushAll();

            headersCache.AssertWasCalled(x => x.Eject(hash1));
            headersCache.AssertWasCalled(x => x.Eject(hash2));
            headersCache.AssertWasCalled(x => x.Eject(hash3));

            theOutputCache.AssertWasCalled(x => x.Eject(hash1));
            theOutputCache.AssertWasCalled(x => x.Eject(hash2));
            theOutputCache.AssertWasCalled(x => x.Eject(hash3));
        }

        [Test]
        public void Changed_will_also_eject_the_header_values_out_of_the_header_cache()
        {
            var hash1 = "12345";
            var hash2 = "23456";

            theCache.LinkFilesToResource(hash1, new AssetFile[]{file1, file2, file3});
            theCache.LinkFilesToResource(hash2, new AssetFile[] { file2, file3, file4 });
            theCache.Changed(file1);

            headersCache.AssertWasCalled(x => x.Eject(hash1));
            theOutputCache.AssertWasCalled(x => x.Eject(hash1));

            theCache.Changed(file2);
        }

        [Test]
        public void Changed_will_also_eject_the_header_values_out_of_the_header_cache_for_every_linked_hash()
        {
            var hash1 = "12345";
            var hash2 = "23456";

            theCache.LinkFilesToResource(hash1, new AssetFile[] { file1, file2, file3 });
            theCache.LinkFilesToResource(hash2, new AssetFile[] { file2, file3, file4 });
            theCache.Changed(file1);

            

            theCache.Changed(file2);

            headersCache.AssertWasCalled(x => x.Eject(hash1));
            headersCache.AssertWasCalled(x => x.Eject(hash2));
            theOutputCache.AssertWasCalled(x => x.Eject(hash1));
            theOutputCache.AssertWasCalled(x => x.Eject(hash2));
        }



    }
}