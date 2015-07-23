using System;
using System.Linq;
using System.Threading.Tasks;
using FubuMVC.Core.Runtime;
using FubuMVC.Tests.TestSupport;
using Shouldly;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Runtime
{
    [TestFixture]
    public class AsyncCoordinatorTester : InteractionContext<AsyncCoordinator>
    {
        protected override void beforeEach()
        {
            Services.Inject(typeof(IExceptionHandler), new HandleInvalidOperationException());
        }

        [Test]
        public void when_an_error_is_handled_complete_is_called()
        {
            MockFor<IRequestCompletion>().Expect(x => x.Complete());
            var task = new Task(() => { throw new InvalidOperationException(); });
            ClassUnderTest.Push(task);
            task.RunSynchronously();
            VerifyCallsFor<IRequestCompletion>();
        }

        [Test]
        public void when_an_error_is_not_handled_complete_with_errors_is_called()
        {
            MockFor<IRequestCompletion>()
                .Expect(x => x.CompleteWithErrors(Arg<AggregateException>.Matches(ex => ex.InnerExceptions.Any(y => y.Message == "not handled"))));
            var task = new Task(() => { throw new Exception("not handled"); });
            ClassUnderTest.Push(task);
            task.RunSynchronously();
            VerifyCallsFor<IRequestCompletion>();
        }

        [Test]
        public void when_no_errors_occur_complete_is_called()
        {
            MockFor<IRequestCompletion>().Expect(x => x.Complete());
            var task = new Task(() => { });
            ClassUnderTest.Push(task);
            task.RunSynchronously();
            VerifyCallsFor<IRequestCompletion>();
        }

        [Test]
        public void when_multiple_errors_occur_and_not_all_are_handled_complete_with_errors_is_called()
        {
            MockFor<IRequestCompletion>()
                .Expect(x => x.CompleteWithErrors(Arg<AggregateException>.Matches(ex => ex.InnerExceptions.Any(y => y.Message == "not handled"))));
            var task = new Task(() => { throw new Exception("not handled"); });
            var task2 = new Task(() => { throw new InvalidOperationException();});
            ClassUnderTest.Push(task);
            ClassUnderTest.Push(task2);
            task.RunSynchronously();
            task2.RunSynchronously();
            VerifyCallsFor<IRequestCompletion>();
        }
    }

    public class HandleInvalidOperationException : IExceptionHandler
    {
        public bool ShouldHandle(Exception exception)
        {
            return exception is InvalidOperationException;
        }

        public void Handle(Exception exception)
        {
        }
    }
}