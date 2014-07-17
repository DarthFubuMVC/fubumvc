using System;
using FubuMVC.Core.Diagnostics.Runtime;
using FubuMVC.Core.Diagnostics.Runtime.Tracing;
using FubuMVC.Tests.Registration;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Diagnostics.Tests.Runtime
{
    [TestFixture]
    public class BehaviorFinishTester
    {
        [Test]
        public void is_successful_by_default()
        {
            new BehaviorFinish(null).Succeeded.ShouldBeTrue();
        }

        [Test]
        public void is_successful_after_capturing_an_exception()
        {
            var correlation = new BehaviorFinish(new BehaviorCorrelation(new FakeNode()));

            var ex = new NotImplementedException("What?");
            correlation.LogException(ex);

            correlation.Succeeded.ShouldBeFalse();
            correlation.Exception.ShouldNotBeNull();
        }
    }
}