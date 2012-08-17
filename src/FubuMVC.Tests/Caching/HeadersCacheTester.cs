using System;
using FubuMVC.Core.Http.Headers;
using FubuMVC.Core.Resources.Etags;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;

namespace FubuMVC.Tests.Caching
{
    [TestFixture]
    public class HeadersCacheTester
    {
        [Test]
        public void current_returns_empty_enumerable_if_nothing()
        {
            new HeadersCache().Current(Guid.NewGuid().ToString()).Any().ShouldBeFalse();
            new HeadersCache().Current(Guid.NewGuid().ToString()).Any().ShouldBeFalse();
            new HeadersCache().Current(Guid.NewGuid().ToString()).Any().ShouldBeFalse();
        }

        [Test]
        public void register_and_fetch()
        {
            var headers = new Header[]{new Header("a", "1"), new Header("b", "2"), new Header("c", "3")};

            var hash = Guid.NewGuid().ToString();

            var cache = new HeadersCache();
            cache.Register(hash, headers);
        
            cache.Current(hash).ShouldHaveTheSameElementsAs(headers);
        }

        [Test]
        public void register_saves_a_copy_of_the_enumerable_and_not_the_same()
        {
            var headers = new Header[] { new Header("a", "1"), new Header("b", "2"), new Header("c", "3") };

            var hash = Guid.NewGuid().ToString();

            var cache = new HeadersCache();
            cache.Register(hash, headers);

            cache.Current(hash).ShouldNotBeTheSameAs(headers);
        }

        [Test]
        public void eject_clears_it_out()
        {
            var headers = new Header[] { new Header("a", "1"), new Header("b", "2"), new Header("c", "3") };

            var hash = Guid.NewGuid().ToString();

            var cache = new HeadersCache();
            cache.Register(hash, headers);

            cache.Eject(hash);

            cache.Current(hash).Any().ShouldBeFalse();
        }
    }
}