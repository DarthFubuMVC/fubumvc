using System.Threading;
using FubuMVC.Diagnostics.Runtime;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Diagnostics.Tests.Runtime
{
    [TestFixture]
    public class TimedReportTester
    {
        [Test]
        public void start_and_end_results_in_a_non_zero_execution_time()
        {
            var report = new TimedReport();
            report.ExecutionTime.ShouldEqual(0);
            Thread.Sleep(100);

            report.MarkFinished();

            report.ExecutionTime.ShouldBeGreaterThan(1);
        }
    }
}