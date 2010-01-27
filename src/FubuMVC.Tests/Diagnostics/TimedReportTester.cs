using System.Threading;
using FubuMVC.Core.Diagnostics;
using NUnit.Framework;

namespace FubuMVC.Tests.Diagnostics
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