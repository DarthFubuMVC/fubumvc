using System.Collections.Generic;
using FubuMVC.Core;
using FubuMVC.Core.Http.Headers;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Resources.Etags;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Tests.Resources.Etags
{
    [TestFixture]
    public class EtagCacheTester
    {
        private EtagCache theCache;

        [SetUp]
        public void SetUp()
        {
            theCache = new EtagCache();
        }

        [Test]
        public void current_returns_null_if_there_is_not_one_without_failing()
        {
            theCache.Current("something").ShouldBeNull();
        }

        [Test]
        public void current_returns_the_current_after_a_register()
        {
            theCache.Register("something", "12345", new List<Header>());

            theCache.Current("something").ShouldEqual("12345");

            theCache.Register("something", "23456", new List<Header>());

            theCache.Current("something").ShouldEqual("23456");

            theCache.Register("else", "567", new List<Header>());
            theCache.Current("else").ShouldEqual("567");
        }

        [Test]
        public void ejecting_an_etag_resets_the_current_to_null()
        {
            theCache.Register("something", "12345", new List<Header>());
            theCache.Eject("something");

            theCache.Current("something").ShouldBeNull();
        }

        [Test]
        public void etag_cache_should_be_registered_as_a_singleton()
        {
            ServiceRegistry.ShouldBeSingleton(typeof(EtagCache))
                .ShouldBeTrue();

            BehaviorGraph.BuildEmptyGraph()
                .Services
                .DefaultServiceFor<IEtagCache>()
                .Type.ShouldEqual(typeof (EtagCache));
        }
    }
}