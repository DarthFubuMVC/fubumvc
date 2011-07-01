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
        private CurrentRequest _request;
        private DiagnosticsConfiguration _configuration;

        protected override void beforeEach()
        {
            _request = new CurrentRequest();
            _configuration = new DiagnosticsConfiguration {MaxRequests = 60};

            Container.Inject(_request);
            Container.Inject(_configuration);
        }

        [Test]
        public void only_keeps_maximum_records()
        {
            MockFor<IRequestHistoryCacheFilter>()
                .Expect(c => c.Exclude(_request))
                .Return(false)
                .Repeat
                .Any();

            for (int i = 0; i < _configuration.MaxRequests + 10; ++i)
            {
                ClassUnderTest.AddReport(new DebugReport());
            }

            ClassUnderTest
                .RecentReports()
                .ShouldHaveCount(_configuration.MaxRequests);
        }

        [Test]
        public void keep_the_newest_reports()
        {
            MockFor<IRequestHistoryCacheFilter>()
                .Expect(c => c.Exclude(_request))
                .Return(false)
                .Repeat
                .Any();

            for (int i = 0; i < _configuration.MaxRequests; i++)
            {
                ClassUnderTest.AddReport(new DebugReport());
            }

            var report1 = new DebugReport();
            var report2 = new DebugReport();
            var report3 = new DebugReport();

            ClassUnderTest.AddReport(report1);
            ClassUnderTest.AddReport(report2);
            ClassUnderTest.AddReport(report3);

            ClassUnderTest
                .RecentReports()
                .Take(3)
                .ShouldHaveTheSameElementsAs(report3, report2, report1);
        }

        [Test]
        public void should_not_add_report_if_any_filter_excludes()
        {
            MockFor<IRequestHistoryCacheFilter>()
                .Expect(c => c.Exclude(_request))
                .Return(true);

            ClassUnderTest.AddReport(new DebugReport());

            ClassUnderTest
                .RecentReports()
                .ShouldHaveCount(0);
        }
    }
}