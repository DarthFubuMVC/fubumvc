using FubuMVC.Core.Diagnostics;
using FubuTestingSupport;
using NUnit.Framework;
using System.Linq;

namespace FubuMVC.Tests.Diagnostics
{
    [TestFixture]
    public class RequestHistoryServiceTester
    {
        [Test]
        public void only_keeps_50_records()
        {
            var history = new RequestHistoryCache();
            for (int i = 0; i < 60; i++)
            {
                history.AddReport(new DebugReport());
            }

            history.RecentReports().ShouldHaveCount(50);
        }

        [Test]
        public void keep_the_newest_reports()
        {
            var history = new RequestHistoryCache();
            for (int i = 0; i < 50; i++)
            {
                history.AddReport(new DebugReport());
            }

            var report1 = new DebugReport();
            var report2 = new DebugReport();
            var report3 = new DebugReport();
        
            history.AddReport(report1);
            history.AddReport(report2);
            history.AddReport(report3);

            history.RecentReports().Take(3).ShouldHaveTheSameElementsAs(report3, report2, report1);
        }
    }
}