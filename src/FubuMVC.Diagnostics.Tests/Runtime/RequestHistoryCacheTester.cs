using FubuMVC.Core;
using FubuMVC.Diagnostics.Runtime;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;

namespace FubuMVC.Diagnostics.Tests.Runtime
{
    [TestFixture]
    public class RequestHistoryCacheTester
    {
        [Test]
        public void only_cache_up_to_the_setting_limit()
        {
            var settings = new DiagnosticsSettings{
                MaxRequests = 10
            };

            var cache = new RequestHistoryCache(settings);

            cache.AddReport(new DebugReport());
            cache.AddReport(new DebugReport());
            cache.AddReport(new DebugReport());
            cache.AddReport(new DebugReport());
            cache.AddReport(new DebugReport());
            cache.AddReport(new DebugReport());
            cache.AddReport(new DebugReport());
            cache.AddReport(new DebugReport());
            cache.AddReport(new DebugReport());

            cache.RecentReports().Count().ShouldEqual(9);

            cache.AddReport(new DebugReport());
            cache.AddReport(new DebugReport());
            cache.AddReport(new DebugReport());
            cache.AddReport(new DebugReport());
            cache.AddReport(new DebugReport());

            cache.RecentReports().Count().ShouldEqual(settings.MaxRequests);
        }
    }
}