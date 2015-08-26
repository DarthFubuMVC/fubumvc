using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics.Instrumentation;
using FubuMVC.Core.Diagnostics.Runtime;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.Diagnostics.Instrumentation
{
    [TestFixture]
    public class ChainExecutionHistoryTester
    {
        [Test]
        public void only_cache_up_to_the_setting_limit()
        {
            var settings = new DiagnosticsSettings
            {
                MaxRequests = 10
            };

            var cache = new ChainExecutionHistory(settings);

            cache.Store(new ChainExecutionLog());
            cache.Store(new ChainExecutionLog());
            cache.Store(new ChainExecutionLog());
            cache.Store(new ChainExecutionLog());
            cache.Store(new ChainExecutionLog());
            cache.Store(new ChainExecutionLog());
            cache.Store(new ChainExecutionLog());
            cache.Store(new ChainExecutionLog());
            cache.Store(new ChainExecutionLog());

            cache.RecentReports().Count().ShouldBe(9);

            cache.Store(new ChainExecutionLog());
            cache.Store(new ChainExecutionLog());
            cache.Store(new ChainExecutionLog());
            cache.Store(new ChainExecutionLog());
            cache.Store(new ChainExecutionLog());

            cache.RecentReports().Count().ShouldBe(settings.MaxRequests);
        }
    }
}