using System;
using FubuMVC.Core;
using FubuMVC.Core.Assets.Caching;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration;
using NUnit.Framework;
using FubuTestingSupport;

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

        private IRecordedOutput getOutputWithEtag(string etag)
        {
            var output = new RecordedOutput();
            output.AppendHeader(HttpResponseHeaders.ETag, etag);

            return output;
        }

        private IRecordedOutput storeAgainstResource(string resourceHash)
        {
            var output = getOutputWithEtag(Guid.NewGuid().ToString());
            theCache.Retrieve(resourceHash, () => output);

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

            theCache = new AssetContentCache();

            theGraph = new FubuRegistry().BuildGraph();
        }

        [Test]
        public void retrieve_with_an_empty_cache_calls_to_the_cache_miss_on_the_first_pass()
        {
            var output1 = getOutputWithEtag("12345");

            theCache.Retrieve(resource1, () => output1).ShouldBeTheSameAs(output1);
        }

        [Test]
        public void retrieve_on_a_cache_hit()
        {
            var output1 = getOutputWithEtag("12345");
            var output2 = getOutputWithEtag("12346");

            theCache.Retrieve(resource1, () => output1).ShouldBeTheSameAs(output1);
            theCache.Retrieve(resource1, () => output2).ShouldBeTheSameAs(output1);
        }

        [Test]
        public void current_etag_on_initial_request_is_null()
        {
            theCache.Current(resource1).ShouldBeNull();
        }

        [Test]
        public void current_etag_after_capturing_a_cache_hit()
        {
            string etag = "12345";
            var output1 = getOutputWithEtag(etag);
            theCache.Retrieve(resource1, () => output1);

            theCache.Current(resource1).ShouldEqual(etag);
        }

        [Test]
        public void link_files_the_clear_cache()
        {
            theCache.LinkFilesToResource(resource1, new AssetFile[]{file1, file2});
            theCache.LinkFilesToResource(resource2, new AssetFile[]{file1, file2, file3});
            theCache.LinkFilesToResource(resource3, new AssetFile[]{file2, file3, file4});

            var output1A = storeAgainstResource(resource1);
            var output2A = storeAgainstResource(resource2);
            var output3A = storeAgainstResource(resource3);

            Func<IRecordedOutput> shouldNotBeCalled = () =>
            {
                Assert.Fail("Do not call me");
                return null;
            };

            // 2nd pass
            theCache.Retrieve(resource1, shouldNotBeCalled).ShouldBeTheSameAs(output1A);
            theCache.Retrieve(resource2, shouldNotBeCalled).ShouldBeTheSameAs(output2A);
            theCache.Retrieve(resource3, shouldNotBeCalled).ShouldBeTheSameAs(output3A);

            theCache.Changed(file1);

            var output1B = getOutputWithEtag("2345");
            var output2B = getOutputWithEtag("23456");
            var output3B = getOutputWithEtag("23457");

            theCache.Retrieve(resource1, () => output1B).ShouldBeTheSameAs(output1B);
            theCache.Retrieve(resource1, shouldNotBeCalled).ShouldBeTheSameAs(output1B);
            theCache.Retrieve(resource1, shouldNotBeCalled).ShouldBeTheSameAs(output1B);

            theCache.Retrieve(resource2, () => output2B).ShouldBeTheSameAs(output2B);
            theCache.Current(resource2).ShouldEqual("23456");

            // Was not cleared because it does not depend on file1
            theCache.Retrieve(resource3, shouldNotBeCalled).ShouldBeTheSameAs(output3A);
        }

        [Test]
        public void is_registered_as_the_asset_file_watcher()
        {
            theGraph.Services.DefaultServiceFor<IAssetFileChangeListener>()
                .Value.ShouldBeOfType<AssetContentCache>();
        }


    }
}