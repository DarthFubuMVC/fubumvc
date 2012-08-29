using System;
using FubuMVC.Core.Runtime.Logging;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Tests.Logging
{
    [TestFixture]
    public class ExceptionReportTester
    {
        [Test]
        public void build_by_exception()
        {
            var exception = new NotImplementedException("What?");

            var report = new ExceptionReport(exception);

            report.Message.ShouldEqual(exception.Message);
            report.ExceptionText.ShouldEqual(exception.ToString());
            report.ExceptionType = "NotImplementedException";
        }

        [Test]
        public void build_by_exception_and_message()
        {
            var exception = new NotImplementedException("What?");

            var report = new ExceptionReport("Something went wrong!", exception);

            report.Message.ShouldEqual("Something went wrong!");
            report.ExceptionText.ShouldEqual(exception.ToString());
            report.ExceptionType = "NotImplementedException";
        }
    }
}