using System;
using FubuCore;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Http;
using FubuMVC.Core.Resources.Etags;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Logging;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Caching
{
    [TestFixture]
    public class when_creating_output_with_an_etag : InteractionContext<OutputCachingBehavior>
    {
        private StubOutputWriter theWriter;
        private readonly string theResource = Guid.NewGuid().ToString();
        private IRecordedOutput theResultingOutput;

        protected override void beforeEach()
        {
            theWriter = new StubOutputWriter();
            Services.Inject<IOutputWriter>(theWriter);

            theWriter.Output.AppendHeader(HttpResponseHeaders.ETag, "12345");

            theResultingOutput = ClassUnderTest.CreateOuput(theResource, x => x.Invoke());
        }

        [Test]
        public void should_have_registered_the_new_etag()
        {
            MockFor<IEtagCache>().AssertWasCalled(x => x.Register(theResource, "12345"));
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

        protected override void beforeEach()
        {
            theCache = new StubOutputCache(MockFor<IRecordedOutput>()){
                CacheHits = true
            };
            Services.Inject<IOutputCache>(theCache);

            MockFor<ICurrentChain>().Stub(x => x.ResourceHash())
                .Return(theResource);

            ClassUnderTest.Invoke();
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

        protected override void beforeEach()
        {
            theCache = new StubOutputCache(MockFor<IRecordedOutput>())
            {
                CacheHits = false
            };
            Services.Inject<IOutputCache>(theCache);

            MockFor<ICurrentChain>().Stub(x => x.ResourceHash())
                .Return(theResource);

            Services.PartialMockTheClassUnderTest();
            theGeneratedOutput = new RecordedOutput(null);
            ClassUnderTest.Expect(x => x.CreateOuput(theResource, ClassUnderTest.Invoker))
                .Return(theGeneratedOutput);

            ClassUnderTest.Invoke();
        }

        [Test]
        public void should_have_written_out_the_recorded_output_generated_from_the_inner_behavior()
        {
            MockFor<IOutputWriter>().AssertWasCalled(x => x.Replay(theGeneratedOutput));
        }
    }

    public class StubOutputWriter : OutputWriter
    {
        public StubOutputWriter() : base(new NulloHttpWriter(), null, new RecordingLogger())
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
    }
}