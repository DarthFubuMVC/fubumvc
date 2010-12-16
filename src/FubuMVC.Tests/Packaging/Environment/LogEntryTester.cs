using FubuMVC.Core.Packaging;
using FubuMVC.Core.Packaging.Environment;
using NUnit.Framework;

namespace FubuMVC.Tests.Packaging.Environment
{
    [TestFixture]
    public class LogEntryTester
    {
        [Test]
        public void from_package_log_successful()
        {
            var packageLog = new PackageRegistryLog();
            packageLog.Trace("some stuff");
            packageLog.Success.ShouldBeTrue();

            var log = LogEntry.FromPackageLog(this, packageLog);
            log.Success.ShouldBeTrue();
            log.TraceText.ShouldEqual(packageLog.FullTraceText().Trim());
            log.Description.ShouldEqual(this.ToString());
        }

        [Test]
        public void from_package_log_failure()
        {
            var packageLog = new PackageRegistryLog();
            packageLog.Trace("some stuff");
            packageLog.MarkFailure("it broke");
            packageLog.Success.ShouldBeFalse();

            var log = LogEntry.FromPackageLog(this, packageLog);
            log.Success.ShouldBeFalse();
            log.TraceText.ShouldEqual(packageLog.FullTraceText().Trim());
            log.Description.ShouldEqual(this.ToString());
        }
    }
}