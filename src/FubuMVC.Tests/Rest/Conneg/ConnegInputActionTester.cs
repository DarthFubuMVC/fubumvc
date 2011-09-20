using System;
using System.Net;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Http;
using FubuMVC.Core.Rest.Conneg;
using FubuMVC.Core.Rest.Media;
using FubuMVC.Core.Runtime;
using FubuMVC.Tests.Rest.Projections;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Rest.Conneg
{
    [TestFixture]
    public class ConnegInputActionTester : InteractionContext<ConnegInputBehavior<Address>>
    {
        private IMediaReader<Address> readerFor(params string[] mimeTypes)
        {
            var reader = Services.AddAdditionalMockFor<IMediaReader<Address>>();
            reader.Stub(x => x.Mimetypes).Return(mimeTypes);

            return reader;
        }

        [Test]
        public void select_reader_simple()
        {
            var reader1 = readerFor("text/json", "application/json");
            var reader2 = readerFor("text/xml");
            var reader3 = readerFor("text/xml", "application/xml");
            var reader4 = readerFor("text/html");

            ClassUnderTest.ChooseReader(new CurrentMimeType("text/json", ""))
                .ShouldBeTheSameAs(reader1);

            ClassUnderTest.ChooseReader(new CurrentMimeType("text/html", ""))
                .ShouldBeTheSameAs(reader4);

            ClassUnderTest.ChooseReader(new CurrentMimeType("application/xml", ""))
                .ShouldBeTheSameAs(reader3);

            ClassUnderTest.ChooseReader(new CurrentMimeType("text/xml", ""))
                .ShouldBeTheSameAs(reader2);
        }

        [Test]
        public void select_reader_complex()
        {
            var reader1 = readerFor("text/json", "application/json");
            var reader2 = readerFor("text/xml");
            var reader3 = readerFor("text/xml", "application/xml");
            var reader4 = readerFor("text/html");

            ClassUnderTest.ChooseReader(new CurrentMimeType("text/json", ""))
                .ShouldBeTheSameAs(reader1);



            ClassUnderTest.ChooseReader(new CurrentMimeType("text/html", ""))
                .ShouldBeTheSameAs(reader4);
        }

        // 415

        
    }

    [TestFixture]
    public class when_executing_the_conneg_input_action_successfully : InteractionContext<ConnegInputBehavior<Address>>
    {
        private IMediaReader<Address> reader1;
        private IMediaReader<Address> reader2;
        private IMediaReader<Address> reader3;
        private IMediaReader<Address> reader4;
        private CurrentMimeType theMimetypes;
        private Address theAddress;
        private IActionBehavior theInnerBehavior;

        private IMediaReader<Address> readerFor(params string[] mimeTypes)
        {
            var reader = Services.AddAdditionalMockFor<IMediaReader<Address>>();
            reader.Stub(x => x.Mimetypes).Return(mimeTypes);

            return reader;
        }

        protected override void beforeEach()
        {
            reader1 = readerFor("text/json", "application/json");
            reader2 = readerFor("text/xml");
            reader3 = readerFor("text/xml", "application/xml");
            reader4 = readerFor("text/html");

            theAddress = new Address();
            reader4.Stub(x => x.Read("text/html")).Return(theAddress);

            theInnerBehavior = MockFor<IActionBehavior>();
            ClassUnderTest.InsideBehavior = theInnerBehavior;

            theMimetypes = new CurrentMimeType("text/html", "");
            MockFor<IFubuRequest>().Stub(x => x.Get<CurrentMimeType>()).Return(theMimetypes);

            

            ClassUnderTest.Invoke();
        }

        [Test]
        public void the_next_behavior_should_have_been_called()
        {
            theInnerBehavior.AssertWasCalled(x => x.Invoke());
        }

        [Test]
        public void the_address_should_have_been_stored_in_the_fubu_request()
        {
            MockFor<IFubuRequest>().AssertWasCalled(x => x.Set(theAddress));
        }
    }

    [TestFixture]
    public class when_executing_the_conneg_input_action_for_an_unknown_content_type : InteractionContext<ConnegInputBehavior<Address>>
    {
        private IMediaReader<Address> reader1;
        private IMediaReader<Address> reader2;
        private IMediaReader<Address> reader3;
        private IMediaReader<Address> reader4;
        private CurrentMimeType theMimetypes;
        private IActionBehavior theInnerBehavior;

        private IMediaReader<Address> readerFor(params string[] mimeTypes)
        {
            var reader = Services.AddAdditionalMockFor<IMediaReader<Address>>();
            reader.Stub(x => x.Mimetypes).Return(mimeTypes);

            return reader;
        }

        protected override void beforeEach()
        {
            reader1 = readerFor("text/json", "application/json");
            reader2 = readerFor("text/xml");
            reader3 = readerFor("text/xml", "application/xml");
            reader4 = readerFor("text/html");

            theInnerBehavior = MockFor<IActionBehavior>();
            ClassUnderTest.InsideBehavior = theInnerBehavior;

            theMimetypes = new CurrentMimeType("something/weird", "");
            MockFor<IFubuRequest>().Stub(x => x.Get<CurrentMimeType>()).Return(theMimetypes);

            ClassUnderTest.Invoke();
        }

        [Test]
        public void the_next_behavior_should_not_have_been_called()
        {
            theInnerBehavior.AssertWasNotCalled(x => x.Invoke());
        }

        [Test]
        public void the_status_code_was_set_to_415()
        {
            MockFor<IOutputWriter>().AssertWasCalled(x => x.WriteResponseCode(HttpStatusCode.UnsupportedMediaType));
        }

        [Test]
        public void the_address_should_not_have_been_stored_in_the_fubu_request()
        {
            MockFor<IFubuRequest>().AssertWasNotCalled(x => x.Set<Address>(null), x => x.IgnoreArguments());
        }
    }
}