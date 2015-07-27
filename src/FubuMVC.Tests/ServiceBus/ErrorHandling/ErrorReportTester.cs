using System;
using FubuMVC.Core.ServiceBus.ErrorHandling;
using FubuMVC.Core.ServiceBus.Runtime;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.ErrorHandling
{
    [TestFixture]
    public class when_creating_an_error_report_from_the_exception_and_envelope
    {
        Envelope envelope = ObjectMother.Envelope();
        Exception exception  = new NotImplementedException();
        private ErrorReport report;

        [SetUp]
        public void SetUp()
        {
            envelope.Message = new Message1();
            report = new ErrorReport(envelope, exception);
            
        }

        [Test]
        public void capture_the_exception_message()
        {
            report.ExceptionMessage.ShouldBe(exception.Message);
        }

        [Test]
        public void capture_the_headers()
        {
            report.Headers.ShouldBe(envelope.Headers.ToNameValues());
        }

        [Test]
        public void explanation_is_via_an_exception()
        {
            report.Explanation.ShouldBe(ErrorReport.ExceptionDetected);
        }

        [Test]
        public void exception_stack_trace_is_captured()
        {
            report.ExceptionText.ShouldBe(exception.ToString());
        }

        [Test]
        public void exception_type_is_captured()
        {
            report.ExceptionType.ShouldBe(exception.GetType().FullName);
        }

        [Test]
        public void can_dehydrate_and_rehydrate_itself()
        {
            var data = report.Serialize();
            var report2 = ErrorReport.Deserialize(data);

            report2.Explanation.ShouldBe(report.Explanation);
            report2.ExceptionMessage.ShouldBe(report.ExceptionMessage);
            report2.ExceptionType.ShouldBe(report.ExceptionType);
            report2.RawData.ShouldBe(report.RawData);

            foreach (string key in report.Headers.Keys)
            {
                report2.Headers.Get(key).ShouldBe(report.Headers.Get(key));
            }
        }
    }
}