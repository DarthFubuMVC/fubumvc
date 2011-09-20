using FubuMVC.Core.Diagnostics;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Diagnostics
{
    [TestFixture]
    public class RequestObserverTester : InteractionContext<RequestObserver>
    {
        [Test]
        public void should_add_log_details()
        {
            var msg = "Test";
            MockFor<IDebugReport>()
                .Expect(r => r.AddDetails(new RequestLogEntry { Message = msg }));

            ClassUnderTest
                .RecordLog(msg);

            VerifyCallsFor<IDebugReport>();
        }

        [Test]
        public void should_add_log_details_with_substitutions()
        {
            var msg = "Test 123";
            MockFor<IDebugReport>()
                .Expect(r => r.AddDetails(new RequestLogEntry { Message = msg }));

            ClassUnderTest
                .RecordLog("Test {0}", "123");

            VerifyCallsFor<IDebugReport>();
        }
    }
}