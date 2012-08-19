using System.Net;
using FubuCore.Dates;
using FubuMVC.Core.Assets;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;
using FubuCore;

namespace FubuMVC.Tests.Assets
{
    [TestFixture]
    public class AssetCacheHeadersTester
    {
        [Test]
        public void sets_cache_control_to_24_hours_by_default()
        {
            var header = new AssetCacheHeaders(SystemTime.Default()).HeadersFor(null).Single(x => x.Name == "Cache-Control");

            header.Name.ShouldEqual("Cache-Control");
            header.Value.ShouldEqual("private, max-age=86400");
        }

        [Test]
        public void configure_a_different_length()
        {
            var headers = new AssetCacheHeaders(SystemTime.Default())
            {
                MaxAgeInSeconds = 11111
            };
            var header = headers.HeadersFor(null).Single(x => x.Name == "Cache-Control");

            header.Value.ShouldEqual("private, max-age=11111");
        }

        [Test]
        public void writes_expires_for_24_hours_by_default()
        {
            var systemTime = SystemTime.AtLocalTime("0800".ToTime());

            var headers = new AssetCacheHeaders(systemTime);

            var header = headers.HeadersFor(null).Single(x => x.Name == "Expires");

            header.Value.ShouldEqual(systemTime.UtcNow().AddDays(1).ToString("R"));
        }

        [Test]
        public void set_the_max_age_to_a_longer_time_frame()
        {
            var systemTime = SystemTime.AtLocalTime("0800".ToTime());

            var headers = new AssetCacheHeaders(systemTime);
            headers.MaxAgeInSeconds = 3*24*60*60;

            var header = headers.HeadersFor(null).Single(x => x.Name == "Expires");

            header.Value.ShouldEqual(systemTime.UtcNow().AddDays(3).ToString("R"));
        }

    }
}