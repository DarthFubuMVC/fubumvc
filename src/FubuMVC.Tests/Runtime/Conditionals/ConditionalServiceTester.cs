using System;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Conditionals;
using FubuMVC.Tests.TestSupport;
using Shouldly;
using Xunit;
using Rhino.Mocks;

namespace FubuMVC.Tests.Behaviors.Conditional
{
    
    public class ConditionalServiceTester : InteractionContext<ConditionalService>
    {
        [Fact]
        public void throw_argument_out_of_range_if_passing_in_a_not_conditional_type()
        {
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() =>
            {
                ClassUnderTest.IsTrue(GetType());
            });
        }

        [Fact]
        public void always_is_true()
        {
            ClassUnderTest.IsTrue(typeof(Always))
                .ShouldBeTrue();
        }

        [Fact]
        public void never_is_false()
        {
            ClassUnderTest.IsTrue(typeof(Never))
                .ShouldBeFalse();
        }

        [Fact]
        public void is_true_positive()
        {
            MockFor<IServiceLocator>().Stub(x => x.GetInstance(typeof(Always)))
                .Return(Always.Flyweight);
        }


        [Fact]
        public void is_true_negative()
        {
            MockFor<IServiceLocator>().Stub(x => x.GetInstance(typeof(Never)))
                .Return(new Never());
        }

        [Fact]
        public void is_true_test_is_memoized()
        {
            var condition = MockRepository.GenerateMock<IConditional>();
            var context = MockFor<IFubuRequestContext>();

            MockFor<IServiceLocator>().Stub(x => x.GetInstance(typeof(FakeConditional)))
                .Return(condition);

            condition.Stub(x => x.ShouldExecute(context)).Return(true);

            ClassUnderTest.IsTrue(typeof(FakeConditional)).ShouldBeTrue();
            ClassUnderTest.IsTrue(typeof(FakeConditional)).ShouldBeTrue();
            ClassUnderTest.IsTrue(typeof(FakeConditional)).ShouldBeTrue();
            ClassUnderTest.IsTrue(typeof(FakeConditional)).ShouldBeTrue();
            ClassUnderTest.IsTrue(typeof(FakeConditional)).ShouldBeTrue();
            ClassUnderTest.IsTrue(typeof(FakeConditional)).ShouldBeTrue();

            condition.AssertWasCalled(x => x.ShouldExecute(context)
            , x => x.Repeat.Once());
        }
    }

    public class FakeConditional : IConditional
    {
        public bool ShouldExecute(IFubuRequestContext context)
        {
            throw new NotImplementedException();
        }
    }


}