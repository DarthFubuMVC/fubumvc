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
        private DiagnosticsSettings _settings;

        protected override void beforeEach()
        {
            theCurrentRequest = new CurrentRequest();


            _settings = new DiagnosticsSettings {MaxRequests = 60};

            Container.Inject(theCurrentRequest);
            Container.Inject(_settings);
        }

        [Test]
        public void only_keeps_maximum_records()
        {
            MockFor<IRequestHistoryCacheFilter>()
                .Expect(c => c.Exclude(theCurrentRequest))
                .Return(false)
                .Repeat
                .Any();

            for (int i = 0; i < _settings.MaxRequests + 10; ++i)
            {
                ClassUnderTest.AddReport(new DebugReport(null, null), theCurrentRequest);
            }

            ClassUnderTest
                .RecentReports()
                .ShouldHaveCount(_settings.MaxRequests);
        }

        [Test]
        public void keep_the_newest_reports()
        {
            MockFor<IRequestHistoryCacheFilter>()
                .Expect(c => c.Exclude(theCurrentRequest))
                .Return(false)
                .Repeat
                .Any();

            for (int i = 0; i < _settings.MaxRequests; i++)
            {
                ClassUnderTest.AddReport(new DebugReport(null, null), theCurrentRequest);
            }

            var report1 = new DebugReport(null, null);
            var report2 = new DebugReport(null, null);
            var report3 = new DebugReport(null, null);

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

            ClassUnderTest.AddReport(new DebugReport(null, null), theCurrentRequest);

            ClassUnderTest
                .RecentReports()
                .ShouldHaveCount(0);
        }
    }
}