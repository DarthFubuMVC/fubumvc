using System;
using FubuCore;
using FubuMVC.Core.Behaviors.Conditional;
using FubuMVC.Core.Runtime.Conditionals;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Behaviors.Conditional
{
    [TestFixture]
    public class ConditionalServiceTester : InteractionContext<ConditionalService>
    {
        [Test]
        public void throw_argument_out_of_range_if_passing_in_a_not_conditional_type()
        {
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() =>
            {
                ClassUnderTest.IsTrue(GetType());
            });
        }

        [Test]
        public void always_is_true()
        {
            ClassUnderTest.IsTrue(typeof(Always))
                .ShouldBeTrue();
        }

        [Test]
        public void is_true_positive()
        {
            MockFor<IServiceLocator>().Stub(x => x.GetInstance(typeof (Always)))
                .Return(Always.Flyweight);
        }


        [Test]
        public void is_true_negative()
        {
            MockFor<IServiceLocator>().Stub(x => x.GetInstance(typeof(Never)))
                .Return(new Never());
        }

        [Test]
        public void is_true_test_is_memoized()
        {
            var condition = MockRepository.GenerateMock<IConditional>();

            MockFor<IServiceLocator>().Stub(x => x.GetInstance(typeof (FakeConditional)))
                .Return(condition);

            condition.Stub(x => x.ShouldExecute()).Return(true);

            ClassUnderTest.IsTrue(typeof(FakeConditional)).ShouldBeTrue();
            ClassUnderTest.IsTrue(typeof(FakeConditional)).ShouldBeTrue();
            ClassUnderTest.IsTrue(typeof(FakeConditional)).ShouldBeTrue();
            ClassUnderTest.IsTrue(typeof(FakeConditional)).ShouldBeTrue();
            ClassUnderTest.IsTrue(typeof(FakeConditional)).ShouldBeTrue();
            ClassUnderTest.IsTrue(typeof(FakeConditional)).ShouldBeTrue();

            condition.AssertWasCalled(x => x.ShouldExecute(), x => x.Repeat.Once());
        }
    }

    public class FakeConditional : IConditional
    {
        public bool ShouldExecute()
        {
            throw new NotImplementedException();
        }
    }


}