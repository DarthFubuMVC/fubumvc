using System;
using System.Data;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.ErrorHandling;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;
using FubuMVC.Tests.TestSupport;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.ErrorHandling
{
    [TestFixture]
    public class ExceptionHandlerBehaviorTester : InteractionContext<ExceptionHandlerBehavior>
    {
        private Envelope theEnvelope;
        private HandlerChain theChain;

        protected override void beforeEach()
        {
            theEnvelope = ObjectMother.Envelope();
            theChain = new HandlerChain();
            
            Services.Inject(theEnvelope);
            Services.Inject(theChain);
        }

        private void theInnerBehaviorThrows<T>() where T : Exception, new()
        {
            var ex = new T();

            MockFor<IActionBehavior>().Expect(x => x.Invoke()).Throw(ex);
        }

        private void theInnerBehaviorThrowsAggregateExceptionWith<T1, T2, T3>()
            where T1 : Exception, new()
            where T2 : Exception, new()
            where T3 : Exception, new()
        {
            var exception = new AggregateException(new T1(), new T2(), new T3());

            MockFor<IActionBehavior>().Expect(x => x.Invoke()).Throw(exception);
        }

        private void theInnerBehaviorSucceeds()
        {
            // just to make the tests prettier
        }

        private IContinuation theContinuationSetByTheErrorHandler()
        {
            var argumentsForCallsMadeOn = MockFor<IInvocationContext>().GetArgumentsForCallsMadeOn(x => x.Continuation = null);
            return argumentsForCallsMadeOn
                       [0][0] as IContinuation;
        }

        [Test]
        public void do_nothing_if_the_inner_behavior_succeeds()
        {
            theInnerBehaviorSucceeds();

            ClassUnderTest.Invoke();

            MockFor<IInvocationContext>().AssertWasNotCalled(x => x.Continuation = null, x => x.IgnoreArguments());
        }

        [Test]
        public void invoke_partial_just_delegates()
        {
            // hey, this has to work
            ClassUnderTest.InvokePartial();

            MockFor<IActionBehavior>().AssertWasCalled(x => x.InvokePartial());
        }

        [Test]
        public void if_attempts_are_equal_to_the_max_and_there_is_a_failure_immediately_send_to_error_report()
        {
            theInnerBehaviorThrows<NotImplementedException>();

            theEnvelope.Attempts = 2;
            theChain.MaximumAttempts = 2;

            ClassUnderTest.Invoke();

            var continuation = theContinuationSetByTheErrorHandler()
                .ShouldBeOfType<MoveToErrorQueue>();

            continuation.Exception.ShouldBeOfType<NotImplementedException>();
        }


        [Test]
        public void if_attempts_are_equal_to_the_max_and_there_is_a_failure_immediately_send_to_error_report_2()
        {
            theInnerBehaviorThrows<NotSupportedException>();

            theEnvelope.Attempts = 2;
            theChain.MaximumAttempts = 2;

            ClassUnderTest.Invoke();

            var continuation = theContinuationSetByTheErrorHandler()
                .ShouldBeOfType<MoveToErrorQueue>();

            continuation.Exception.ShouldBeOfType<NotSupportedException>();
        }

        [Test]
        public void if_attempts_are_under_the_threshold_try_the_exception_rules()
        {
            theInnerBehaviorThrows<NotImplementedException>();

            theEnvelope.Attempts = 1;
            theChain.MaximumAttempts = 3;

            var rule = new FakeExceptionRule<NotImplementedException>();
            theChain.ErrorHandlers.Add(rule);

            ClassUnderTest.Invoke();

            theContinuationSetByTheErrorHandler().ShouldBeTheSameAs(rule.Continuation);
        }

        [Test]
        public void chooses_the_first_non_null_continuation_from_an_error_handler()
        {
            var rule1 = new FakeExceptionRule<NotImplementedException>();
            var rule2 = new FakeExceptionRule<NotSupportedException>();
            var rule3 = new FakeExceptionRule<NotFiniteNumberException>();

            theEnvelope.Attempts = 1;
            theChain.MaximumAttempts = 3;

            theChain.ErrorHandlers.Add(rule1);
            theChain.ErrorHandlers.Add(rule2);
            theChain.ErrorHandlers.Add(rule3);

            theInnerBehaviorThrows<NotImplementedException>();

            ClassUnderTest.Invoke();

            theContinuationSetByTheErrorHandler().ShouldBeTheSameAs(rule1.Continuation);
        }

        [Test]
        public void chooses_the_first_non_null_continuation_from_an_error_handler_2()
        {
            var rule1 = new FakeExceptionRule<NotImplementedException>();
            var rule2 = new FakeExceptionRule<NotSupportedException>();
            var rule3 = new FakeExceptionRule<NotFiniteNumberException>();

            theEnvelope.Attempts = 1;
            theChain.MaximumAttempts = 3;

            theChain.ErrorHandlers.Add(rule1);
            theChain.ErrorHandlers.Add(rule2);
            theChain.ErrorHandlers.Add(rule3);

            theInnerBehaviorThrows<NotSupportedException>();

            ClassUnderTest.Invoke();

            theContinuationSetByTheErrorHandler().ShouldBeTheSameAs(rule2.Continuation);
        }

        [Test]
        public void chooses_the_first_non_null_continuation_from_an_error_handler_3()
        {
            var rule1 = new FakeExceptionRule<NotImplementedException>();
            var rule2 = new FakeExceptionRule<NotSupportedException>();
            var rule3 = new FakeExceptionRule<NotFiniteNumberException>();

            theEnvelope.Attempts = 1;
            theChain.MaximumAttempts = 3;

            theChain.ErrorHandlers.Add(rule1);
            theChain.ErrorHandlers.Add(rule2);
            theChain.ErrorHandlers.Add(rule3);

            theInnerBehaviorThrows<NotFiniteNumberException>();

            ClassUnderTest.Invoke();

            theContinuationSetByTheErrorHandler().ShouldBeTheSameAs(rule3.Continuation);
        }

        [Test]
        public void none_of_the_rules_match_so_it_goes_to_the_error_queue()
        {
            var rule1 = new FakeExceptionRule<NotImplementedException>();
            var rule2 = new FakeExceptionRule<NotSupportedException>();
            var rule3 = new FakeExceptionRule<NotFiniteNumberException>();

            theEnvelope.Attempts = 1;
            theChain.MaximumAttempts = 3;

            theChain.ErrorHandlers.Add(rule1);
            theChain.ErrorHandlers.Add(rule2);
            theChain.ErrorHandlers.Add(rule3);

            theInnerBehaviorThrows<Exception>();

            ClassUnderTest.Invoke();

            var continuation = theContinuationSetByTheErrorHandler()
                .ShouldBeOfType<MoveToErrorQueue>();

            continuation.Exception.ShouldBeOfType<Exception>();
        }

        //==================Aggregate Exceptions================================
        [Test]
        public void if_attempts_are_under_the_threshold_try_the_exception_rules_with_aggregate()
        {
            theInnerBehaviorThrowsAggregateExceptionWith<NullReferenceException, NotImplementedException, NotSupportedException>();

            theEnvelope.Attempts = 1;
            theChain.MaximumAttempts = 3;

            var rule = new FakeExceptionRule<NotImplementedException>();
            theChain.ErrorHandlers.Add(rule);

            ClassUnderTest.Invoke();

            theContinuationSetByTheErrorHandler().ShouldBeTheSameAs(rule.Continuation);
        }

        [Test]
        public void chooses_the_first_non_null_continuation_from_an_error_handler_for_the_first_matching_in_the_aggregate()
        {
            var rule1 = new FakeExceptionRule<NotImplementedException>();
            var rule2 = new FakeExceptionRule<NotSupportedException>();
            var rule3 = new FakeExceptionRule<NotFiniteNumberException>();

            theEnvelope.Attempts = 1;
            theChain.MaximumAttempts = 3;

            theChain.ErrorHandlers.Add(rule1);
            theChain.ErrorHandlers.Add(rule2);
            theChain.ErrorHandlers.Add(rule3);

            theInnerBehaviorThrowsAggregateExceptionWith<NullReferenceException, NotImplementedException, Exception>();


            ClassUnderTest.Invoke();

            theContinuationSetByTheErrorHandler().ShouldBeTheSameAs(rule1.Continuation);
        }

        [Test]
        public void chooses_the_first_non_null_continuation_from_an_error_handler_for_the_first_matching_in_the_aggregate_2()
        {
            var rule1 = new FakeExceptionRule<NotImplementedException>();
            var rule2 = new FakeExceptionRule<NotSupportedException>();
            var rule3 = new FakeExceptionRule<NotFiniteNumberException>();

            theEnvelope.Attempts = 1;
            theChain.MaximumAttempts = 3;

            theChain.ErrorHandlers.Add(rule1);
            theChain.ErrorHandlers.Add(rule2);
            theChain.ErrorHandlers.Add(rule3);

            theInnerBehaviorThrowsAggregateExceptionWith<Exception, NullReferenceException, NotImplementedException>();


            ClassUnderTest.Invoke();

            theContinuationSetByTheErrorHandler().ShouldBeTheSameAs(rule1.Continuation);
        }

        [Test]
        public void chooses_the_first_non_null_continuation_from_an_error_handler_2_for_aggregate()
        {
            var rule1 = new FakeExceptionRule<NotImplementedException>();
            var rule2 = new FakeExceptionRule<NotSupportedException>();
            var rule3 = new FakeExceptionRule<NotFiniteNumberException>();

            theEnvelope.Attempts = 1;
            theChain.MaximumAttempts = 3;

            theChain.ErrorHandlers.Add(rule1);
            theChain.ErrorHandlers.Add(rule2);
            theChain.ErrorHandlers.Add(rule3);

            theInnerBehaviorThrowsAggregateExceptionWith<Exception, ApplicationException, NotSupportedException>();

            ClassUnderTest.Invoke();

            theContinuationSetByTheErrorHandler().ShouldBeTheSameAs(rule2.Continuation);
        }

        [Test]
        public void chooses_the_first_non_null_continuation_from_an_error_handler_3_with_aggregate()
        {
            var rule1 = new FakeExceptionRule<NotImplementedException>();
            var rule2 = new FakeExceptionRule<NotSupportedException>();
            var rule3 = new FakeExceptionRule<NotFiniteNumberException>();

            theEnvelope.Attempts = 1;
            theChain.MaximumAttempts = 3;

            theChain.ErrorHandlers.Add(rule1);
            theChain.ErrorHandlers.Add(rule2);
            theChain.ErrorHandlers.Add(rule3);

            theInnerBehaviorThrowsAggregateExceptionWith<Exception, NotFiniteNumberException, ApplicationException>();

            ClassUnderTest.Invoke();

            theContinuationSetByTheErrorHandler().ShouldBeTheSameAs(rule3.Continuation);
        }

        [Test]
        public void none_of_the_rules_match_so_it_goes_to_the_error_queue_with_aggregate()
        {
            var rule1 = new FakeExceptionRule<NotImplementedException>();
            var rule2 = new FakeExceptionRule<NotSupportedException>();
            var rule3 = new FakeExceptionRule<NotFiniteNumberException>();

            theEnvelope.Attempts = 1;
            theChain.MaximumAttempts = 3;

            theChain.ErrorHandlers.Add(rule1);
            theChain.ErrorHandlers.Add(rule2);
            theChain.ErrorHandlers.Add(rule3);

            theInnerBehaviorThrowsAggregateExceptionWith<Exception, ApplicationException, DBConcurrencyException>();

            ClassUnderTest.Invoke();

            var continuation = theContinuationSetByTheErrorHandler()
                .ShouldBeOfType<MoveToErrorQueue>();

            continuation.Exception.ShouldBeOfType<AggregateException>();
        }

    }

    public class FakeExceptionRule<T> : IErrorHandler where T : Exception
    {
        public readonly IContinuation Continuation = MockRepository.GenerateMock<IContinuation>();

        public IContinuation DetermineContinuation(Envelope envelope, Exception ex)
        {
            return ex is T ? Continuation : null;
        }
    }
}