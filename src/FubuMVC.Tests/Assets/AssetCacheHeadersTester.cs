using System.Net;
using FubuMVC.Core.Assets;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;

namespace FubuMVC.Tests.Assets
{
    [TestFixture]
    public class AssetCacheHeadersTester
    {
        [Test]
        public void sets_cache_control_to_24_hours_by_default()
        {
            var header = new AssetCacheHeaders().Headers(null).Single();

            header.Name.ShouldEqual("Cache-Control");
            header.Value.ShouldEqual("private, max-age=86400");
        }

        [Test]
        public void configure_a_different_length()
        {
            var headers = new AssetCacheHeaders{
                MaxAgeInSeconds = 11111
            };
            var header = headers.Headers(null).Single();

            header.Value.ShouldEqual("private, max-age=11111");
        }

    }
}