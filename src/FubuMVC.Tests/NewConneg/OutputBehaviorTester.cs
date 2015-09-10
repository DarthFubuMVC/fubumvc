using System.Collections.Generic;
using System.Linq;
using System.Net;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Headers;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using FubuMVC.Tests.TestSupport;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.NewConneg
{
    [TestFixture]
    public class when_there_are_outputs_that_would_write_headers : InteractionContext<OutputBehavior<Address>>
    {
        protected override void beforeEach()
        {
            Services.Container.Configure(x => x.For<IFubuRequestContext>().Use<FubuRequestContext>());

            var headers1 = new HttpHeaderValues();
            headers1["a"] = "1";
            headers1["b"] = "2";

            var headers2 = new HttpHeaderValues();
            headers2["c"] = "3";
            headers2["d"] = "4";

            MockFor<IFubuRequest>().Stub(x => x.Find<IHaveHeaders>()).Return(new IHaveHeaders[] {headers1, headers2});

            ClassUnderTest.WriteHeaders();
        }


        [Test]
        public void should_write_all_possible_headers()
        {
            MockFor<IOutputWriter>().AssertWasCalled(x => x.AppendHeader("a", "1"));
            MockFor<IOutputWriter>().AssertWasCalled(x => x.AppendHeader("b", "2"));
            MockFor<IOutputWriter>().AssertWasCalled(x => x.AppendHeader("c", "3"));
            MockFor<IOutputWriter>().AssertWasCalled(x => x.AppendHeader("d", "4"));
        }
    }


    [TestFixture]
    public class when_the_resource_cannot_be_found : InteractionContext<OutputBehavior<OutputTarget>>
    {
        protected override void beforeEach()
        {
            Services.Container.Configure(x => x.For<IFubuRequestContext>().Use<FubuRequestContext>());
            MockFor<IFubuRequest>().Stub(x => x.Get<OutputTarget>()).Return(null);


            ClassUnderTest.Invoke();
        }

        [Test]
        public void should_write_the_404_just_in_case()
        {
            MockFor<IOutputWriter>().AssertWasCalled(x => x.WriteResponseCode(HttpStatusCode.NotFound));
        }

        [Test]
        public void should_call_through_to_the_resource_not_found_handler()
        {
            MockFor<IResourceNotFoundHandler>().AssertWasCalled(x => x.HandleResourceNotFound<OutputTarget>());
        }
    }


    [TestFixture]
    public class when_writing_and_a_media_can_be_found : InteractionContext<OutputBehavior<OutputTarget>>
    {
        private IMediaWriter<OutputTarget> theSelectedMedia;
        private MockedFubuRequestContext theContext;
        private OutputTarget theTarget;
        private CurrentMimeType theMimeType;

        protected override void beforeEach()
        {
            theContext = new MockedFubuRequestContext(Services.Container);
            Services.Inject<IFubuRequestContext>(theContext);

            theTarget = new OutputTarget();

            theMimeType = new CurrentMimeType("text/plain", "text/plain");
            theSelectedMedia = MockFor<IMediaWriter<OutputTarget>>();
            theSelectedMedia.Stub(x => x.Mimetypes).Return(new[] {"text/plain"});

            MockFor<IMediaCollection<OutputTarget>>().Stub(x => x.SelectWriter(theMimeType, theContext))
                .Return(theSelectedMedia);


            ClassUnderTest.WriteResource(theMimeType, theTarget);
        }

        [Test]
        public void should_use_the_selected_media()
        {
            theSelectedMedia.AssertWasCalled(x => x.Write("text/plain", MockFor<IFubuRequestContext>(), theTarget));
        }
    }


    [TestFixture]
    public class when_writing_and_no_matching_writer_can_be_found : InteractionContext<OutputBehavior<OutputTarget>>
    {
        private MockedFubuRequestContext theContext;
        private OutputTarget theTarget;
        private CurrentMimeType theMimeType;

        protected override void beforeEach()
        {
            theContext = new MockedFubuRequestContext(Services.Container);
            Services.Inject<IFubuRequestContext>(theContext);

            theTarget = new OutputTarget();
            theMimeType = new CurrentMimeType("text/plain", "text/plain");

            MockFor<IMediaCollection<OutputTarget>>().Stub(x => x.SelectWriter(theMimeType, theContext)).Return(null);

            ClassUnderTest.WriteResource(theMimeType, theTarget);
        }

        [Test]
        public void should_write_a_406_not_acceptable()
        {
            MockFor<IOutputWriter>().AssertWasCalled(x => x.WriteResponseCode(HttpStatusCode.NotAcceptable));
        }
    }

    [TestFixture]
    public class output_behavior_in_partials : OutputBehaviorContext
    {
        protected override void theContextIs()
        {
            Services.PartialMockTheClassUnderTest();
        }

        [Test]
        public void if_no_output_partial_behavior_in_the_request_invoke()
        {
            MockFor<IFubuRequest>().Stub(x => x.Has<OutputPartialBehavior>()).Return(false);

            ClassUnderTest.Expect(x => x.Write());

            ClassUnderTest.InvokePartial();

            ClassUnderTest.VerifyAllExpectations();
        }

        [Test]
        public void if_output_partial_is_write_invoke()
        {
            MockFor<IFubuRequest>().Stub(x => x.Has<OutputPartialBehavior>()).Return(true);
            MockFor<IFubuRequest>().Stub(x => x.Get<OutputPartialBehavior>()).Return(OutputPartialBehavior.Write);

            ClassUnderTest.Expect(x => x.Write());

            ClassUnderTest.InvokePartial();

            ClassUnderTest.VerifyAllExpectations();
        }

        [Test]
        public void if_output_partial_is_none_do_not_invoke()
        {
            MockFor<IFubuRequest>().Stub(x => x.Has<OutputPartialBehavior>()).Return(true);
            MockFor<IFubuRequest>().Stub(x => x.Get<OutputPartialBehavior>()).Return(OutputPartialBehavior.None);

            ClassUnderTest.Expect(x => x.Write()).Repeat.Never();

            ClassUnderTest.InvokePartial();

            ClassUnderTest.VerifyAllExpectations();
        }
    }

    public abstract class OutputBehaviorContext : InteractionContext<OutputBehavior<OutputTarget>>
    {
        protected IMediaWriter<OutputTarget>[] theMedia;
        protected CurrentMimeType theCurrentMimeType;
        protected OutputTarget theTarget;


        protected override sealed void beforeEach()
        {
            Services.Container.Configure(x => x.For<IFubuRequestContext>().Use<FubuRequestContext>());
            RecordLogging();

            theMedia = Services.CreateMockArrayFor<IMediaWriter<OutputTarget>>(5);

            var output = new OutputNode(typeof (OutputTarget));

            theMedia.Each(media => { output.Add(media); });

            Services.Inject<IMediaCollection<OutputTarget>>(new MediaCollection<OutputTarget>(output));

            theCurrentMimeType = new CurrentMimeType();
            theTarget = new OutputTarget();

            MockFor<IFubuRequest>().Stub(x => x.Get<OutputTarget>()).Return(theTarget);
            MockFor<IFubuRequest>().Stub(x => x.Get<CurrentMimeType>()).Return(theCurrentMimeType);

            MockFor<IFubuRequest>().Stub(x => x.Find<IHaveHeaders>()).Return(new IHaveHeaders[0]);

            theContextIs();
        }

        protected abstract void theContextIs();

        protected void mediaMimetypesAre(int index, params string[] mimeTypes)
        {
            theMedia[index].Stub(x => x.Mimetypes).Return(mimeTypes);
        }

        protected void mediaMimetypesAre(int index, params MimeType[] mimeTypes)
        {
            mediaMimetypesAre(index, mimeTypes.Select(x => x.Value).ToArray());
        }

        protected void theSelectedMediaShouldBe(int index)
        {
            MockFor<IMediaCollection<OutputTarget>>().SelectWriter(theCurrentMimeType, MockFor<IFubuRequestContext>())
                .ShouldBeTheSameAs(theMedia[index]);
        }
    }

    public class OutputTarget
    {
    }
}