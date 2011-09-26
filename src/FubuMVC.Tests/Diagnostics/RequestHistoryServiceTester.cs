using FubuMVC.Core;
using FubuMVC.Core.Diagnostics;
using FubuTestingSupport;
using NUnit.Framework;
using System.Linq;
using Rhino.Mocks;

namespace FubuMVC.Tests.Diagnostics
{
    [TestFixture]
    public class RequestHistoryServiceTester : InteractionContext<RequestHistoryCache>
    {
        private CurrentRequest theCurrentRequest;
        private DiagnosticsConfiguration _configuration;

        protected override void beforeEach()
        {
            theCurrentRequest = new CurrentRequest();


            _configuration = new DiagnosticsConfiguration {MaxRequests = 60};

            Container.Inject(theCurrentRequest);
            Container.Inject(_configuration);
        }

        [Test]
        public void only_keeps_maximum_records()
        {
            MockFor<IRequestHistoryCacheFilter>()
                .Expect(c => c.Exclude(theCurrentRequest))
                .Return(false)
                .Repeat
                .Any();

            for (int i = 0; i < _configuration.MaxRequests + 10; ++i)
            {
                ClassUnderTest.AddReport(new DebugReport(), theCurrentRequest);
            }

            ClassUnderTest
                .RecentReports()
                .ShouldHaveCount(_configuration.MaxRequests);
        }

        [Test]
        public void keep_the_newest_reports()
        {
            MockFor<IRequestHistoryCacheFilter>()
                .Expect(c => c.Exclude(theCurrentRequest))
                .Return(false)
                .Repeat
                .Any();

            for (int i = 0; i < _configuration.MaxRequests; i++)
            {
                ClassUnderTest.AddReport(new DebugReport(), theCurrentRequest);
            }

            var report1 = new DebugReport();
            var report2 = new DebugReport();
            var report3 = new DebugReport();

            ClassUnderTest.AddReport(report1, theCurrentRequest);
            ClassUnderTest.AddReport(report2, theCurrentRequest);
            ClassUnderTest.AddReport(report3, theCurrentRequest);

            ClassUnderTest
                .RecentReports()
                .Take(3)
                .ShouldHaveTheSameElementsAs(report3, report2, report1);
        }

        [Test]
        public void should_not_add_report_if_any_filter_excludes()
        {
            MockFor<IRequestHistoryCacheFilter>()
                .Expect(c => c.Exclude(theCurrentRequest))
                .Return(true);

            ClassUnderTest.AddReport(new DebugReport(), theCurrentRequest);

            ClassUnderTest
                .RecentReports()
                .ShouldHaveCount(0);
        }
    }
}