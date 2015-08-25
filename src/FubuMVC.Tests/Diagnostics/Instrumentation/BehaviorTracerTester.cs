using System;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Diagnostics.Instrumentation;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Tests.Registration;
using FubuMVC.Tests.TestSupport;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.Diagnostics.Instrumentation
{
    [TestFixture]
    public class BehaviorTracerTester : InteractionContext<FubuMVC.Core.Diagnostics.Instrumentation.BehaviorTracer>
    {
        [Test]
        public void when_the_inner_behavior_runs_cleanly()
        {
            var node = Wrapper.For<FakeBehavior>();
            Services.Inject<BehaviorNode>(node);

            var inner = MockFor<IActionBehavior>();
            ClassUnderTest.Inner = inner;

            ClassUnderTest.Invoke();

            var log = MockFor<IChainExecutionLog>();
            log.AssertWasCalled(x => x.StartSubject(node));

            inner.AssertWasCalled(x => x.Invoke());

            log.AssertWasCalled(x => x.FinishSubject());

        }

        [Test]
        public void when_the_inner_behavior_throws_an_exception()
        {
            var node = Wrapper.For<FakeBehavior>();
            Services.Inject<BehaviorNode>(node);

            var ex = new DivideByZeroException();
            var inner = MockFor<IActionBehavior>();
            inner.Stub(x => x.Invoke()).Throw(ex);

            ClassUnderTest.Inner = inner;

            Exception<DivideByZeroException>.ShouldBeThrownBy(() =>
            {
                ClassUnderTest.Invoke();
            });

            

            var log = MockFor<IChainExecutionLog>();
            log.AssertWasCalled(x => x.StartSubject(node));

            inner.AssertWasCalled(x => x.Invoke());
            log.AssertWasCalled(x => x.LogException(ex));

            log.AssertWasCalled(x => x.FinishSubject());
        }
    }
}