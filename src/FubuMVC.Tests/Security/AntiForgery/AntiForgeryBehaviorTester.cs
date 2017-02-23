using System.Net;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security.AntiForgery;
using FubuMVC.Tests.TestSupport;
using Shouldly;
using Xunit;
using Rhino.Mocks;

namespace FubuMVC.Tests.Security.AntiForgery
{
    
    public class when_validator_passes : InteractionContext<AntiForgeryBehavior>
    {
        protected override void beforeEach()
        {
            MockFor<IAntiForgeryValidator>().Stub(v => v.Validate("salt")).Return(true);
            Container.Configure(c => c.For<AntiForgeryBehavior>().Use<AntiForgeryBehavior>().Ctor<string>().Is("salt"));
            ClassUnderTest.InsideBehavior = MockFor<IActionBehavior>();
            ClassUnderTest.Invoke();
        }

        [Fact]
        public void should_invoke_next()
        {
            ClassUnderTest.InsideBehavior.AssertWasCalled(b => b.Invoke());
        }

        [Fact]
        public void should_not_write_to_output()
        {
            MockFor<IOutputWriter>().AssertWasNotCalled(o => o.WriteResponseCode(HttpStatusCode.InternalServerError));
        }
    }

    
    public class when_validator_fails : InteractionContext<AntiForgeryBehavior>
    {
        protected override void beforeEach()
        {
            MockFor<IAntiForgeryValidator>().Stub(v => v.Validate("salt")).Return(false);
            Container.Configure(c => c.For<AntiForgeryBehavior>().Use<AntiForgeryBehavior>().Ctor<string>().Is("salt"));
            ClassUnderTest.InsideBehavior = MockFor<IActionBehavior>();
            ClassUnderTest.Invoke();
        }

        [Fact]
        public void should_not_invoke_next()
        {
            ClassUnderTest.InsideBehavior.AssertWasNotCalled(b => b.Invoke());
        }

        [Fact]
        public void should_not_write_to_output()
        {
            MockFor<IOutputWriter>().AssertWasCalled(o => o.WriteResponseCode(HttpStatusCode.InternalServerError));
        }
    }
}