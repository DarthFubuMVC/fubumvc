using System;
using System.Net;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using FubuMVC.Tests.Registration;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Continuations
{
    [TestFixture]
    public class FubuContinuationTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            director = MockRepository.GenerateMock<IContinuationDirector>();
        }

        #endregion

        private IContinuationDirector director;

        private void shouldFail(Action action)
        {
            Exception<FubuAssertionException>.ShouldBeThrownBy(action);
        }

        [Test]
        public void Continue_just_continues()
        {
            FubuContinuation continuation = FubuContinuation.NextBehavior();
            continuation.Type.ShouldEqual(ContinuationType.NextBehavior);

            continuation.Process(director);
            director.AssertWasCalled(x => x.InvokeNextBehavior());
        }

        [Test]
        public void end_with_status_code_calls_to_same_on_continuation_director()
        {
            var continuation = FubuContinuation.EndWithStatusCode(HttpStatusCode.NotModified);

            continuation.Type.ShouldEqual(ContinuationType.Stop);
            continuation.Process(director);

            director.AssertWasCalled(x => x.EndWithStatusCode(HttpStatusCode.NotModified));
        }


        [Test]
        public void assert_continued()
        {
            FubuContinuation continuation = FubuContinuation.NextBehavior();
            continuation.AssertWasContinuedToNextBehavior();

            shouldFail(() => continuation.AssertWasRedirectedTo("here"));
            shouldFail(() => continuation.AssertWasTransferedTo("here"));
            shouldFail(() => continuation.AssertWasTransferedTo<ControllerTarget>(x => x.OneInOneOut(null)));
            shouldFail(() => continuation.AssertWasRedirectedTo<ControllerTarget>(x => x.OneInOneOut(null)));
        }

        [Test]
        public void assert_redirect_to_a_method()
        {
            FubuContinuation continuation = FubuContinuation.RedirectTo<ControllerTarget>(x => x.OneInOneOut(null));
            continuation.Type.ShouldEqual(ContinuationType.Redirect);

            continuation.AssertWasRedirectedTo<ControllerTarget>(x => x.OneInOneOut(null));

            shouldFail(() => continuation.AssertWasRedirectedTo("here"));
            shouldFail(() => continuation.AssertWasTransferedTo("here"));
            shouldFail(() => continuation.AssertWasTransferedTo<ControllerTarget>(x => x.OneInZeroOut(null)));
            shouldFail(() => continuation.AssertWasRedirectedTo<ControllerTarget>(x => x.OneInZeroOut(null)));
            shouldFail(() => continuation.AssertWasContinuedToNextBehavior());
        }

        [Test]
        public void assert_redirect_to_a_method_with_category()
        {
            FubuContinuation continuation = FubuContinuation.RedirectTo<ControllerTarget>(x => x.OneInOneOut(null));
            continuation.Type.ShouldEqual(ContinuationType.Redirect);

            continuation.AssertWasRedirectedTo<ControllerTarget>(x => x.OneInOneOut(null));

            shouldFail(() => continuation.AssertWasRedirectedTo("here"));
            shouldFail(() => continuation.AssertWasTransferedTo("here"));
            shouldFail(() => continuation.AssertWasTransferedTo<ControllerTarget>(x => x.OneInZeroOut(null)));
            shouldFail(() => continuation.AssertWasRedirectedTo<ControllerTarget>(x => x.OneInZeroOut(null)));
            shouldFail(() => continuation.AssertWasContinuedToNextBehavior());
        }

        [Test]
        public void assert_redirect_to_a_target_via_equals()
        {
            var input = new InputModelWithEquals {Name = "Luke"};
            FubuContinuation continuation = FubuContinuation.RedirectTo(input);

            continuation.AssertWasRedirectedTo(new InputModelWithEquals {Name = "Luke"});

            shouldFail(() => continuation.AssertWasRedirectedTo(new InputModelWithEquals()));
            shouldFail(() => continuation.AssertWasTransferedTo(input));
            shouldFail(() => continuation.AssertWasContinuedToNextBehavior());

            shouldFail(() => continuation.AssertWasTransferedTo<ControllerTarget>(x => x.OneInOneOut(null)));
            shouldFail(() => continuation.AssertWasRedirectedTo<ControllerTarget>(x => x.OneInOneOut(null)));
        }

        [Test]
        public void assert_redirect_to_a_target_via_predicate()
        {
            var input = new InputModel { Name = "Luke", Age = 1 };
            FubuContinuation continuation = FubuContinuation.RedirectTo(input);

            continuation.AssertWasRedirectedTo<InputModel>(x => x.Name == input.Name);

            shouldFail(() => continuation.AssertWasRedirectedTo(new InputModelWithEquals()));
            shouldFail(() => continuation.AssertWasTransferedTo(input));
            shouldFail(() => continuation.AssertWasContinuedToNextBehavior());

            shouldFail(() => continuation.AssertWasTransferedTo<ControllerTarget>(x => x.OneInOneOut(null)));
            shouldFail(() => continuation.AssertWasRedirectedTo<ControllerTarget>(x => x.OneInOneOut(null)));
        }

        [Test]
        public void when_destination_null_redirect_to_throws()
        {
            var argNullEx = typeof (ArgumentNullException).ShouldBeThrownBy(() => FubuContinuation.RedirectTo(null));
            argNullEx.Message.ShouldContain("destination");
        }

        [Test]
        public void when_destination_null_transfer_to_throws()
        {
            var argNullEx = typeof (ArgumentNullException).ShouldBeThrownBy(() => FubuContinuation.TransferTo(null));
            argNullEx.Message.ShouldContain("destination");
        }

        [Test]
        public void assert_transfer_to_a_method()
        {
            FubuContinuation continuation = FubuContinuation.TransferTo<ControllerTarget>(x => x.OneInOneOut(null));

            continuation.AssertWasTransferedTo<ControllerTarget>(x => x.OneInOneOut(null));

            shouldFail(() => continuation.AssertWasRedirectedTo("here"));
            shouldFail(() => continuation.AssertWasTransferedTo("here"));
            shouldFail(() => continuation.AssertWasTransferedTo<ControllerTarget>(x => x.OneInZeroOut(null)));
            shouldFail(() => continuation.AssertWasRedirectedTo<ControllerTarget>(x => x.OneInZeroOut(null)));
            shouldFail(() => continuation.AssertWasContinuedToNextBehavior());
        }

        [Test]
        public void assert_transfered_to_by_method_respects_category()
        {
            var continuation = FubuContinuation.TransferTo<ControllerTarget>(x => x.OneInOneOut(null), "PUT");

            continuation.AssertWasTransferedTo<ControllerTarget>(x => x.OneInOneOut(null), "PUT");

            shouldFail(() => continuation.AssertWasTransferedTo<ControllerTarget>(x => x.OneInOneOut(null), "GET"));
            shouldFail(() => continuation.AssertWasTransferedTo<ControllerTarget>(x => x.OneInOneOut(null)));
        }

        [Test]
        public void assert_transfer_by_destination_respects_category()
        {
            var input = new InputModel { Name = "Luke", Age = 1 };
            var continuation = FubuContinuation.TransferTo(input, "PUT");

            continuation.AssertWasTransferedTo(input, "PUT");

            shouldFail(() => continuation.AssertWasTransferedTo(input));
            shouldFail(() => continuation.AssertWasTransferedTo(input, "GET"));
            
        }

        [Test]
        public void assert_was_redirected_to_destination_respects_category()
        {
            var input = new InputModel { Name = "Luke", Age = 1 };
            var continuation = FubuContinuation.RedirectTo(input, "PUT");

            continuation.AssertWasRedirectedTo(input, "PUT");

            shouldFail(() => continuation.AssertWasRedirectedTo(input));
            shouldFail(() => continuation.AssertWasRedirectedTo(input, "GET"));
        }

        [Test]
        public void assert_redirected_to_by_method_respects_category()
        {
            var continuation = FubuContinuation.RedirectTo<ControllerTarget>(x => x.OneInOneOut(null), "PUT");

            continuation.AssertWasRedirectedTo<ControllerTarget>(x => x.OneInOneOut(null), "PUT");

            shouldFail(() => continuation.AssertWasRedirectedTo<ControllerTarget>(x => x.OneInOneOut(null), "GET"));
            shouldFail(() => continuation.AssertWasRedirectedTo<ControllerTarget>(x => x.OneInOneOut(null)));
        }

        [Test]
        public void assert_transfer_to_a_target_via_equals()
        {
            var input = new InputModelWithEquals {Name = "Luke"};
            FubuContinuation continuation = FubuContinuation.TransferTo(input);

            continuation.AssertWasTransferedTo(new InputModelWithEquals {Name = "Luke"});

            shouldFail(() => continuation.AssertWasTransferedTo(new InputModelWithEquals()));
            shouldFail(() => continuation.AssertWasRedirectedTo(input));
            shouldFail(() => continuation.AssertWasContinuedToNextBehavior());

            shouldFail(() => continuation.AssertWasTransferedTo<ControllerTarget>(x => x.OneInOneOut(null)));
            shouldFail(() => continuation.AssertWasRedirectedTo<ControllerTarget>(x => x.OneInOneOut(null)));
        }

        [Test]
        public void assert_transfer_to_a_target_via_predicate()
        {
            var input = new InputModel {Name = "Luke", Age = 1};
            FubuContinuation continuation = FubuContinuation.TransferTo(input);

            continuation.AssertWasTransferedTo<InputModel>(x => x.Name == input.Name);

            shouldFail(() => continuation.AssertWasTransferedTo(new InputModelWithEquals()));
            shouldFail(() => continuation.AssertWasRedirectedTo(input));
            shouldFail(() => continuation.AssertWasContinuedToNextBehavior());

            shouldFail(() => continuation.AssertWasTransferedTo<ControllerTarget>(x => x.OneInOneOut(null)));
            shouldFail(() => continuation.AssertWasRedirectedTo<ControllerTarget>(x => x.OneInOneOut(null)));
        }

        [Test]
        public void redirect_to_a_method()
        {
            FubuContinuation continuation = FubuContinuation.RedirectTo<ControllerTarget>(x => x.OneInOneOut(null));

            continuation.Type.ShouldEqual(ContinuationType.Redirect);

            continuation.Process(director);

            ActionCall call = ActionCall.For<ControllerTarget>(x => x.OneInOneOut(null));

            director.AssertWasCalled(x => x.RedirectToCall(call));
        }

        [Test]
        public void redirect_to_a_method_with_category()
        {
            FubuContinuation continuation = FubuContinuation.RedirectTo<ControllerTarget>(x => x.OneInOneOut(null), "PUT");

            continuation.Type.ShouldEqual(ContinuationType.Redirect);

            continuation.Process(director);

            ActionCall call = ActionCall.For<ControllerTarget>(x => x.OneInOneOut(null));

            director.AssertWasCalled(x => x.RedirectToCall(call, "PUT"));
        }

        [Test]
        public void redirect_to_a_target()
        {
            var input = new InputModel();
            FubuContinuation continuation = FubuContinuation.RedirectTo(input);

            continuation.Type.ShouldEqual(ContinuationType.Redirect);

            continuation.Process(director);

            director.AssertWasCalled(x => x.RedirectTo(input));
        }

        [Test]
        public void redirect_to_a_target_with_category()
        {
            var input = new InputModel();
            FubuContinuation continuation = FubuContinuation.RedirectTo(input, "PUT");

            continuation.Type.ShouldEqual(ContinuationType.Redirect);

            continuation.Process(director);

            director.AssertWasCalled(x => x.RedirectTo(input, "PUT"));
        }


        [Test]
        public void transfer_to_a_method()
        {
            FubuContinuation continuation = FubuContinuation.TransferTo<ControllerTarget>(x => x.OneInOneOut(null));

            continuation.Type.ShouldEqual(ContinuationType.Transfer);

            continuation.Process(director);

            ActionCall call = ActionCall.For<ControllerTarget>(x => x.OneInOneOut(null));

            director.AssertWasCalled(x => x.TransferToCall(call));
        }

        [Test]
        public void transfer_to_a_method_with_a_category()
        {
            FubuContinuation continuation = FubuContinuation.TransferTo<ControllerTarget>(x => x.OneInOneOut(null), "PUT");

            continuation.Type.ShouldEqual(ContinuationType.Transfer);

            continuation.Process(director);

            ActionCall call = ActionCall.For<ControllerTarget>(x => x.OneInOneOut(null));

            director.AssertWasCalled(x => x.TransferToCall(call, "PUT"));
        }

        [Test]
        public void transfer_to_a_target()
        {
            var input = new InputModel();
            FubuContinuation continuation = FubuContinuation.TransferTo(input);

            continuation.Type.ShouldEqual(ContinuationType.Transfer);

            continuation.Process(director);

            director.AssertWasCalled(x => x.TransferTo(input));
        }

        [Test]
        public void transfer_to_a_target_with_a_method()
        {
            var input = new InputModel();
            FubuContinuation continuation = FubuContinuation.TransferTo(input, "PUT");

            continuation.Type.ShouldEqual(ContinuationType.Transfer);

            continuation.Process(director);

            director.AssertWasCalled(x => x.TransferTo(input, "PUT"));
        }

        [Test]
        public void transfer_to_a_method_with_a_category_2()
        {
            FubuContinuation continuation = FubuContinuation.TransferTo<ControllerTarget>(x => x.OneInOneOut(null), "PUT");

            continuation.Type.ShouldEqual(ContinuationType.Transfer);

            continuation.Process(director);

            ActionCall call = ActionCall.For<ControllerTarget>(x => x.OneInOneOut(null));

            director.AssertWasCalled(x => x.TransferToCall(call, "PUT"));
        }

        [Test]
        public void transfer_to_null_throws()
        {
            var urlRegistry = MockRepository.GenerateStub<IUrlRegistry>();
            var outputWriter = MockRepository.GenerateStub<IOutputWriter>();
            var fubuRequest = MockRepository.GenerateStub<IFubuRequest>();
            var partialFactory = MockRepository.GenerateStub<IPartialFactory>();
            var resolver = MockRepository.GenerateStub<IChainResolver>();

            var handler = new ContinuationHandler(urlRegistry, outputWriter, fubuRequest, partialFactory, resolver);

            var exception =
                typeof (ArgumentNullException).ShouldBeThrownBy(() => handler.TransferTo(null)) as ArgumentNullException;
            exception.ShouldNotBeNull();
            exception.ParamName.ShouldEqual("input");
        }


        [Test]
        public void destination_returns_destination_object()
        {
            var destination = new object();
            var continuation = FubuContinuation.TransferTo(destination);
            continuation.Destination<object>().ShouldEqual(destination);
        }

        [Test]
        public void redirect_via_generic()
        {
            var continuation = FubuContinuation.RedirectTo<TestModel>();
            continuation.AssertWasRedirectedTo<TestModel>(x => x.GetType() == typeof (TestModel));
        }

        [Test]
        public void redirect_via_generic_with_a_method()
        {
            var continuation = FubuContinuation.RedirectTo<TestModel>("PUT");
            continuation.AssertWasRedirectedTo<TestModel>(x => x.GetType() == typeof(TestModel), "PUT");
        }

        [Test]
        public void transfer_via_generic()
        {
            var continuation = FubuContinuation.TransferTo<TestModel>();
            continuation.AssertWasTransferedTo<TestModel>(x => x.GetType() == typeof(TestModel));
        }

        [Test]
        public void transfer_via_generic_with_a_method()
        {
            var continuation = FubuContinuation.TransferTo<TestModel>("PUT");
            continuation.AssertWasTransferedTo<TestModel>(x => x.GetType() == typeof(TestModel), "PUT");
        }

        [Test]
        public void assert_exited_with_status_code()
        {
            var continuation = FubuContinuation.EndWithStatusCode(HttpStatusCode.NotModified);

            continuation.AssertWasEndedWithStatusCode(HttpStatusCode.NotModified);


            shouldFail(() => continuation.AssertWasEndedWithStatusCode(HttpStatusCode.UseProxy));
        }

        [Test]
        public void assert_exited_with_status_code_negative()
        {
            var continuation = FubuContinuation.RedirectTo<TestModel>();

            shouldFail(() => continuation.AssertWasEndedWithStatusCode(HttpStatusCode.NotModified));

            
        }

        public class TestModel
        {
            
        }
    }

}