using System;
using System.Threading;
using FubuCore;
using FubuMVC.Core.ServiceBus;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus
{
    [TestFixture, Explicit("Flaky from the command line")]
    public class TimeoutRunnerTester
    {
        [Test]
        public void run_happy_path()
        {
            bool wasCalled = false;

            TimeoutRunner.Run(1.Seconds(), () => wasCalled = true, e => {
                throw e;
            }).ShouldBe(Completion.Success);

            wasCalled.ShouldBeTrue();
        }

        [Test]
        public void run_exception_case()
        {
            var ex = new FubuException(400, "Bad!");

            var handler = MockRepository.GenerateMock<Action<Exception>>();

            TimeoutRunner.Run(1.Seconds(), () => {
                throw ex;   
            },
                handler).ShouldBe(Completion.Exception);
        }

        [Test]
        public void run_timeout()
        {
            TimeoutRunner.Run(1.Seconds(), () => Thread.Sleep(2.Seconds()), ex => Assert.Fail("should be no exception"))
                .ShouldBe(Completion.Timedout);
        }
    }
}