using System;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Http;
using FubuTestingSupport;
using NUnit.Framework;
using System.Linq;

namespace FubuMVC.Tests.Caching
{
    [TestFixture]
    public class OutputCacheTester
    {
        private OutputCache theOutputCache;

        private readonly string resource1 = Guid.NewGuid().ToString();
        private readonly string resource2 = Guid.NewGuid().ToString();
        private readonly string resource3 = Guid.NewGuid().ToString();
        private readonly string resource4 = Guid.NewGuid().ToString();

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
            theOutputCache = new OutputCache();   
        }


        [Test]
        public void retrieve_with_an_empty_cache_calls_to_the_cache_miss_on_the_first_pass()
        {
            var output1 = storeAgainstResource(resource1);

            theOutputCache.Retrieve(resource1, () => output1).ShouldBeTheSameAs(output1);
        }


        [Test]
        public void retrieve_on_a_cache_hit()
        {
            var output1 = storeAgainstResource("12345");
            var output2 = getOutputWithEtag("12346");

            theOutputCache.Retrieve(resource1, () => output1).ShouldBeTheSameAs(output1);
            theOutputCache.Retrieve(resource1, () => output2).ShouldBeTheSameAs(output1);
        }

        [Test]
        public void Eject_clears()
        {
            Func<IRecordedOutput> shouldNotBeCalled = () =>
            {
                Assert.Fail("Do not call me");
                return null;
            };

            var output1 = getOutputWithEtag("12345");
            theOutputCache.Retrieve("12345", () => output1).ShouldBeTheSameAs(output1);
            theOutputCache.Eject("12345");

            var output2 = new RecordedOutput(null);
            theOutputCache.Retrieve("12345", () => output2).ShouldBeTheSameAs(output2);
            theOutputCache.Retrieve("12345", shouldNotBeCalled).ShouldBeTheSameAs(output2);
            theOutputCache.Retrieve("12345", shouldNotBeCalled).ShouldBeTheSameAs(output2);
            theOutputCache.Retrieve("12345", shouldNotBeCalled).ShouldBeTheSameAs(output2);
            theOutputCache.Retrieve("12345", shouldNotBeCalled).ShouldBeTheSameAs(output2);
            theOutputCache.Retrieve("12345", shouldNotBeCalled).ShouldBeTheSameAs(output2);
        }

        [Test]
        public void eject_all()
        {
            theOutputCache.Retrieve(resource1, () => getOutputWithEtag(resource1));
            theOutputCache.Retrieve(resource2, () => getOutputWithEtag(resource2));
            theOutputCache.Retrieve(resource3, () => getOutputWithEtag(resource3));
            theOutputCache.Retrieve(resource4, () => getOutputWithEtag(resource4));

            theOutputCache.AllCachedResources().ShouldHaveCount(4);

            theOutputCache.FlushAll();

            theOutputCache.AllCachedResources().Any().ShouldBeFalse();
        }
    }
}