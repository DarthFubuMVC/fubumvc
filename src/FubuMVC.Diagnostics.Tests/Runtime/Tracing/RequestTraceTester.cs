using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FubuMVC.Core.Diagnostics.Runtime;
using FubuMVC.Core.Diagnostics.Runtime.Tracing;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Headers;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Diagnostics.Tests.Runtime.Tracing
{
    [TestFixture]
    public class RequestTrace_has_a_nullo_request_log_by_default
    {
        [Test]
        public void it_is_a_nullo()
        {
            new RequestTrace(new IRequestTraceObserver[0], null, null)
                .Current.ShouldBeOfType<NulloRequestLog>();
        }

        [Test]
        public void nullo_does_not_record_steps_no_matter_what()
        {
            var trace = new RequestTrace(new IRequestTraceObserver[0], null, null);
            trace.Log("a");
            trace.Log("a");
            trace.Log("a");
            trace.Log("a");
            trace.Log("a");
            trace.Log("a");
            trace.Log("a");
            trace.Log("a");
            trace.Log("a");

            trace.Current.AllSteps().Any().ShouldBeFalse();
        }
    }


    [TestFixture]
    public class when_marking_as_a_Failed_request : InteractionContext<RequestTrace>
    {
        [Test]
        public void mark_as_failed()
        {
            ClassUnderTest.Current = new RequestLog();

            ClassUnderTest.MarkAsFailedRequest();

            ClassUnderTest.Current.Failed.ShouldBeTrue();
        }
    }

    [TestFixture]
    public class when_starting_a_new_request : InteractionContext<RequestTrace>
    {
        private RequestLog theLog;
        private IRequestTraceObserver _requestObserver;

        protected override void beforeEach()
        {
            theLog = new RequestLog();

            MockFor<IRequestLogBuilder>().Stub(x => x.BuildForCurrentRequest())
                .Return(theLog);

            _requestObserver = Services.AddAdditionalMockFor<IRequestTraceObserver>();

            ClassUnderTest.Stopwatch.IsRunning.ShouldBeFalse();

            ClassUnderTest.Start();
        }

        [Test]
        public void starts_a_new_request_log()
        {
            ClassUnderTest.Current.ShouldBeTheSameAs(theLog);
        }

        [Test]
        public void notifies_the_new_log_with_the_cache_on_start()
        {
            _requestObserver.AssertWasCalled(x => x.Started(ClassUnderTest.Current));
        }

        [Test]
        public void starts_the_stopwatch()
        {
            ClassUnderTest.Stopwatch.IsRunning.ShouldBeTrue();
        }
    }

    [TestFixture]
    public class when_logging_a_message : InteractionContext<RequestTrace>
    {
        protected override void beforeEach()
        {
            ClassUnderTest.Current = new RequestLog();

            ClassUnderTest.Stopwatch.Start();
            Thread.Sleep(10);
            ClassUnderTest.Stopwatch.Stop();

            ClassUnderTest.Log("something");
        }

        [Test]
        public void logs_to_the_current_request_log_with_the_stopwatch_time()
        {
            ClassUnderTest.Current.AllSteps().Single()
                .ShouldEqual(new RequestStep(ClassUnderTest.Stopwatch.ElapsedMilliseconds, "something"));
        }
    }

    [TestFixture]
    public class when_marking_the_current_log_as_finished : InteractionContext<RequestTrace>
    {
        private IEnumerable<Header> theHeaders;
        private IRequestTraceObserver _requestObserver;

        protected override void beforeEach()
        {
            _requestObserver = Services.CreateMockArrayFor<IRequestTraceObserver>(1).Single();

            ClassUnderTest.Current = new RequestLog();

            theHeaders = new Header[]
                              {new Header("a", "1"), new Header("b", "2")};
            MockFor<IHttpResponse>().Stub(x => x.AllHeaders()).Return(theHeaders);

            ClassUnderTest.Stopwatch.Start();
            Thread.Sleep(10);

            ClassUnderTest.MarkFinished();
        }

        [Test]
        public void places_all_the_response_headers_onto_the_request_log()
        {
            ClassUnderTest.Current.ResponseHeaders.ShouldHaveTheSameElementsAs(theHeaders);
        }

        [Test]
        public void notifies_the_new_log_with_the_cache_on_end()
        {
            _requestObserver.AssertWasCalled(x => x.Completed(ClassUnderTest.Current));
        }

        [Test]
        public void should_stop_the_stopwatch()
        {
            ClassUnderTest.Stopwatch.IsRunning.ShouldBeFalse();
        }

        [Test]
        public void should_mark_the_execution_time_on_the_RequestLog()
        {
            ClassUnderTest.Current.ExecutionTime.ShouldEqual(ClassUnderTest.Stopwatch.ElapsedMilliseconds);
        }
    }
}