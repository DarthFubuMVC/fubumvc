using System.Net;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Http;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Resources.Media;
using FubuMVC.Core.Runtime;
using FubuMVC.Tests.Resources.Projections;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Resources.Conneg
{
    public class conneg_output_behavior : InteractionContext<ConnegOutputBehavior<Address>>
    {
        protected IMediaWriter<Address> writerFor(params string[] mimeTypes)
        {
            var writer = Services.AddAdditionalMockFor<IMediaWriter<Address>>();
            writer.Stub(x => x.Mimetypes).Return(mimeTypes);
            return writer;
        }        
    }

    [TestFixture]
    public class when_the_accept_type_includes_html_just_pass_on : conneg_output_behavior
    {
        private CurrentMimeType theCurrentMimeType;
        protected override void beforeEach()
        {
            theCurrentMimeType = new CurrentMimeType("somethig", "else,text/html");
            MockFor<IFubuRequest>().Stub(x => x.Get<CurrentMimeType>()).Return(theCurrentMimeType);
            writerFor("text/xml");

            ClassUnderTest.InsideBehavior = MockFor<IActionBehavior>();

            ClassUnderTest.Invoke();
        }

        [Test]
        public void should_call_to_the_next_behavior()
        {
            MockFor<IActionBehavior>().AssertWasCalled(x => x.Invoke());
        }
    }

    [TestFixture]
    public class when_the_accept_type_is_wildcard_just_pass_on : conneg_output_behavior
    {
        private CurrentMimeType theCurrentMimeType;

        protected override void beforeEach()
        {
            theCurrentMimeType = new CurrentMimeType("somethig", "*/*");
            MockFor<IFubuRequest>().Stub(x => x.Get<CurrentMimeType>()).Return(theCurrentMimeType);
            writerFor("text/xml");

            ClassUnderTest.InsideBehavior = MockFor<IActionBehavior>();

            ClassUnderTest.Invoke();
        }

        [Test]
        public void should_call_to_the_next_behavior()
        {
            MockFor<IActionBehavior>().AssertWasCalled(x => x.Invoke());
        }
    }

    [TestFixture]
    public class when_not_able_to_find_a_media_writer_for_media_types_and_not_html : conneg_output_behavior
    {
        private CurrentMimeType theCurrentMimeType;

        protected override void beforeEach()
        {
            theCurrentMimeType = new CurrentMimeType("somethig", "else");
            MockFor<IFubuRequest>().Stub(x => x.Get<CurrentMimeType>()).Return(theCurrentMimeType);
            writerFor("text/xml");

            Services.PartialMockTheClassUnderTest();
            ClassUnderTest.Expect(x => x.SelectWriter(theCurrentMimeType)).Return(null);
            


            ClassUnderTest.InsideBehavior = MockFor<IActionBehavior>();

            ClassUnderTest.Invoke();
        }

        [Test]
        public void should_NOT_call_to_the_next_behavior()
        {
            MockFor<IActionBehavior>().AssertWasNotCalled(x => x.Invoke());
        }

        [Test]
        public void should_set_The_status_code_to_NotAcceptable()
        {
            MockFor<IOutputWriter>().AssertWasCalled(x => x.WriteResponseCode(HttpStatusCode.NotAcceptable));
        }
    }

    [TestFixture]
    public class when_able_to_find_a_media_writer_for_media_types : conneg_output_behavior
    {
        private CurrentMimeType theCurrentMimeType;

        protected override void beforeEach()
        {
            theCurrentMimeType = new CurrentMimeType("somethig", "else");
            MockFor<IFubuRequest>().Stub(x => x.Get<CurrentMimeType>()).Return(theCurrentMimeType);

            Services.CreateMockArrayFor<IMediaWriter<Address>>(3);
            
            
            Services.PartialMockTheClassUnderTest();
            ClassUnderTest.Expect(x => x.SelectWriter(theCurrentMimeType)).Return(MockFor<IMediaWriter<Address>>());



            ClassUnderTest.InsideBehavior = MockFor<IActionBehavior>();


            MockFor<IValueSource<Address>>().Stub(x => x.FindValues()).Return(MockFor<IValues<Address>>());

            ClassUnderTest.Invoke();
        }

        [Test]
        public void should_NOT_call_to_the_next_behavior()
        {
            MockFor<IActionBehavior>().AssertWasNotCalled(x => x.Invoke());
        }

        [Test]
        public void should_not_make_any_changes_to_the_http_status_code()
        {
            MockFor<IOutputWriter>().AssertWasNotCalled(x => x.WriteResponseCode(HttpStatusCode.NotAcceptable), x => x.IgnoreArguments());
        }

        [Test]
        public void should_write_using_the_selected_media_writer()
        {
            var outputWriter = MockFor<IOutputWriter>();
            var values = MockFor<IValues<Address>>();
            MockFor<IMediaWriter<Address>>().AssertWasCalled(x => x.Write(values, outputWriter));
        }
    }



    [TestFixture]
    public class ConnegOutputBehaviorTester : conneg_output_behavior
    {
        protected override void beforeEach()
        {
            
        }

        [Test]
        public void select_writer_simple_case()
        {
            var w1 = writerFor("text/json", "application/json");
            var w2 = writerFor("text/xml");
            var w3 = writerFor("text/html");

            ClassUnderTest.SelectWriter(new CurrentMimeType("", "application/json"))
                .ShouldBeTheSameAs(w1);

        }

        [Test]
        public void select_writer_uses_the_accept_types_in_order()
        {
            var w1 = writerFor("text/json", "application/json");
            var w2 = writerFor("text/xml");
            var w3 = writerFor("text/html");

            ClassUnderTest.SelectWriter(new CurrentMimeType("", "text/html,application/json,text/json"))
                .ShouldBeTheSameAs(w3);
        }

        [Test]
        public void select_writer_goes_thru_progression_of_accept_types_until_it_finds_what_it_wants()
        {
            var w1 = writerFor("text/json", "application/json");
            var w2 = writerFor("text/xml");
            var w3 = writerFor("text/html");

            ClassUnderTest.SelectWriter(new CurrentMimeType("", "weird/html,wild/json,text/json"))
                .ShouldBeTheSameAs(w1);
        }

        [Test]
        public void select_writer_can_return_null_if_no_possible_writers_can_be_found_for_the_accept_types()
        {
            var w1 = writerFor("text/json", "application/json");
            var w2 = writerFor("text/xml");

            ClassUnderTest.SelectWriter(new CurrentMimeType("", "weird/html,wild/json"))
                .ShouldBeNull();
        }
    }
}