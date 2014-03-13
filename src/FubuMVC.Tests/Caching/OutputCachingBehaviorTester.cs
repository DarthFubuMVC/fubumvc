using System;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Logging;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Headers;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Core.Resources.Etags;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Logging;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using Is = Rhino.Mocks.Constraints.Is;

namespace FubuMVC.Tests.Caching
{
    [TestFixture]
    public class when_creating_output_with_an_etag : InteractionContext<OutputCachingBehavior>
    {
        private StubOutputWriter theWriter;
        private readonly string theResource = Guid.NewGuid().ToString();
        private IRecordedOutput theResultingOutput;
        private HeadersCache theHeaders;

        protected override void beforeEach()
        {
            theWriter = new StubOutputWriter();
            Services.Inject<IOutputWriter>(theWriter);

            theWriter.Output.AppendHeader(HttpResponseHeaders.ETag, "12345");
            theWriter.Output.AppendHeader("a", "1");

            theHeaders = new HeadersCache();
            Services.Inject<IHeadersCache>(theHeaders);

            theResultingOutput = ClassUnderTest.CreateOutput(theResource, () => MockFor<IActionBehavior>().Invoke());
        }

        [Test]
        public void registered_all_the_headers()
        {
            theHeaders.Current(theResource).ShouldHaveTheSameElementsAs(new Header(HttpResponseHeaders.ETag, "12345"), new Header("a", "1"));
        }

        [Test]
        public void the_recorded_output_was_returned()
        {
            theResultingOutput.ShouldBeTheSameAs(theWriter.Output);
        }

        [Test]
        public void invoked_the_inner_behavior()
        {
            MockFor<IActionBehavior>().AssertWasCalled(x => x.Invoke());
        }
    }

    [TestFixture]
    public class when_executing_the_behavior_with_a_cache_hit : InteractionContext<OutputCachingBehavior>
    {
        private StubOutputCache theCache;
        private readonly string theResource = Guid.NewGuid().ToString();
        private ILogger theLogger;
        private IDictionary<string, string> theDescription;

        protected override void beforeEach()
        {
            theCache = new StubOutputCache(MockFor<IRecordedOutput>()){
                CacheHits = true
            };
            Services.Inject<IOutputCache>(theCache);

            MockFor<IResourceHash>().Stub(x => x.CreateHash())
                .Return(theResource);

            theDescription = new Dictionary<string, string>();
            MockFor<IResourceHash>().Stub(x => x.Describe()).Return(theDescription);


            theLogger = MockFor<ILogger>();
            Services.Inject<ILogger>(new InteractionContextLogger(theLogger));

            ClassUnderTest.Inner = MockFor<IActionBehavior>();

            ClassUnderTest.Invoke();
        }

        [Test]
        public void should_debug_log_the_cache_hit()
        {
            theLogger.AssertWasCalled(x => x.DebugMessage(new CacheHit()), x => x.Constraints(Is.Matching<CacheHit>(h => h.Description == theDescription)));
        }

        [Test]
        public void should_have_written_out_the_cached_recorded_output()
        {
            MockFor<IOutputWriter>().AssertWasCalled(x => x.Replay(theCache.Output));
        }
    }

    [TestFixture]
    public class when_executing_the_behavior_with_a_cache_miss : InteractionContext<OutputCachingBehavior>
    {
        private StubOutputCache theCache;
        private readonly string theResource = Guid.NewGuid().ToString();
        private IRecordedOutput theGeneratedOutput;
        private ILogger theLogger;
        private Dictionary<string, string> theDescription;

        protected override void beforeEach()
        {
            theCache = new StubOutputCache(MockFor<IRecordedOutput>())
            {
                CacheHits = false
            };
            Services.Inject<IOutputCache>(theCache);


            theDescription = new Dictionary<string, string>();
            MockFor<IResourceHash>().Stub(x => x.Describe()).Return(theDescription);
            MockFor<IResourceHash>().Stub(x => x.CreateHash())
                .Return(theResource);


            theLogger = MockFor<ILogger>();
            Services.Inject<ILogger>(new InteractionContextLogger(theLogger));

            Services.PartialMockTheClassUnderTest();
            theGeneratedOutput = new RecordedOutput(null);



            ClassUnderTest.Expect(x => x.CreateOutput(theResource, null)).Constraints(Is.Same(theResource), Is.Anything())
                .Return(theGeneratedOutput);



            ClassUnderTest.Inner = MockFor<IActionBehavior>();
            ClassUnderTest.Invoke();
        }

        [Test]
        public void should_have_written_out_the_recorded_output_generated_from_the_inner_behavior()
        {
            MockFor<IOutputWriter>().AssertWasCalled(x => x.Replay(theGeneratedOutput));
        }


        [Test]
        public void should_debug_log_the_cache_miss()
        {
            theLogger.AssertWasCalled(x => x.DebugMessage(new CacheMiss()), x => x.Constraints(Is.Matching<CacheMiss>(h => h.Description == theDescription)));
        }
    }

    public class StubOutputWriter : OutputWriter
    {
        public StubOutputWriter() : base(new OwinHttpResponse(), null, new RecordingLogger())
        {
            Output = new RecordedOutput(null);

        }

        public RecordedOutput Output { get; private set; }

        public override IRecordedOutput Record(Action action)
        {
            action();
            return Output;
        }
    }

    public class StubOutputCache : IOutputCache
    {
        private readonly IRecordedOutput _output;

        public StubOutputCache(IRecordedOutput output)
        {
            _output = output;
        }

        public IRecordedOutput Output
        {
            get { return _output; }
        }

        public bool CacheHits { get; set; }

        public IRecordedOutput Retrieve(string resourceHash, Func<IRecordedOutput> cacheMiss)
        {
            if (CacheHits)
            {
                return _output;
            }
            else
            {
                return cacheMiss();
            }


        }

        public void Eject(string resourceHash)
        {
            throw new NotImplementedException();
        }

        public void FlushAll()
        {
            throw new NotImplementedException();
        }
    }

    public class InteractionContextLogger : ILogger
    {
        private readonly ILogger _inner;

        public InteractionContextLogger(ILogger inner)
        {
            _inner = inner;
        }

        public void Debug(string message, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public void Info(string message, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public void Error(string message, Exception ex)
        {
            throw new NotImplementedException();
        }

        public void Error(object correlationId, string message, Exception ex)
        {
            throw new NotImplementedException();
        }

        public void Debug(Func<string> message)
        {
            throw new NotImplementedException();
        }

        public void Info(Func<string> message)
        {
            throw new NotImplementedException();
        }

        public void DebugMessage(LogTopic message)
        {
            throw new NotImplementedException();
        }

        public void InfoMessage(LogTopic message)
        {
            throw new NotImplementedException();
        }

        public void DebugMessage<T>(Func<T> message) where T : class, LogTopic
        {
            _inner.DebugMessage(message());
        }

        public void InfoMessage<T>(Func<T> message) where T : class, LogTopic
        {
            _inner.InfoMessage(message());
        }
    }
}