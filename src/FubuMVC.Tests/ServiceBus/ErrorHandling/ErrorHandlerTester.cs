using System;
using FubuMVC.Core.ServiceBus.ErrorHandling;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;
using Xunit;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.ErrorHandling
{
    
    public class ErrorHandlerTester
    {
        [Fact]
        public void continuation_is_move_to_error_queue_by_default()
        {
            new ErrorHandler().Continuation(null, null)
                              .ShouldBeOfType<RequeueContinuation>();
        }

        [Fact]
        public void matches_with_no_rules_is_true()
        {
            new ErrorHandler().Matches(ObjectMother.Envelope(), new Exception())
                              .ShouldBeTrue();
        }

        [Fact]
        public void if_there_are_conditions_all_conditions_must_be_true_to_match()
        {
            var exception = new Exception();
            var envelope = ObjectMother.Envelope();

            var matchingCondition1 = MockRepository.GenerateMock<IExceptionMatch>();
            var matchingCondition2 = MockRepository.GenerateMock<IExceptionMatch>();
            var matchingCondition3 = MockRepository.GenerateMock<IExceptionMatch>();
            var conditionThatDoesNotMatch = MockRepository.GenerateMock<IExceptionMatch>();

            matchingCondition1.Stub(x => x.Matches(envelope, exception)).Return(true);
            matchingCondition2.Stub(x => x.Matches(envelope, exception)).Return(true);
            matchingCondition3.Stub(x => x.Matches(envelope, exception)).Return(true);
            conditionThatDoesNotMatch.Stub(x => x.Matches(envelope, exception)).Return(false);

            var handler = new ErrorHandler();

            handler.AddCondition(matchingCondition1);
            handler.Matches(envelope, exception).ShouldBeTrue();

            handler.AddCondition(matchingCondition2);
            handler.Matches(envelope, exception).ShouldBeTrue();

            handler.AddCondition(matchingCondition3);
            handler.Matches(envelope, exception).ShouldBeTrue();

            handler.AddCondition(conditionThatDoesNotMatch);
            handler.Matches(envelope, exception).ShouldBeFalse();
        }

        [Fact]
        public void if_nothing_matches_do_not_return_a_continuation()
        {
            var exception = new Exception();
            var envelope = ObjectMother.Envelope();

            var conditionThatDoesNotMatch = MockRepository.GenerateMock<IExceptionMatch>();


            var handler = new ErrorHandler();
            handler.AddCondition(conditionThatDoesNotMatch);

            handler.DetermineContinuation(envelope, exception)
                   .ShouldBeNull();
        }

        [Fact]
        public void return_the_continuation_if_the_handler_matches()
        {
            var exception = new Exception();
            var envelope = ObjectMother.Envelope();

            var matchingCondition1 = MockRepository.GenerateMock<IExceptionMatch>();
            matchingCondition1.Stub(x => x.Matches(envelope, exception)).Return(true);

            var handler = new ErrorHandler();

            handler.AddCondition(matchingCondition1);
            handler.AddContinuation(MockRepository.GenerateMock<IContinuation>());

            handler.Matches(envelope, exception).ShouldBeTrue();

            handler.DetermineContinuation(envelope, exception)
                   .ShouldBeTheSameAs(handler.Continuation(null, null));
        }
    }
}