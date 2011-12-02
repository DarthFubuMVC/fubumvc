using System;
using Fubu.Applications;
using FubuMVC.Core;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using Is = Rhino.Mocks.Constraints.Is;

namespace FubuMVC.Tests.Applications
{
    [TestFixture]
    public class when_starting_an_application_runner_and_the_source_cannot_be_resolved :
        InteractionContext<ApplicationRunner>
    {
        private ApplicationSettings theSettings;
        private ApplicationStartResponse theResponse;


        protected override void beforeEach()
        {
            theSettings = new ApplicationSettings();

            MockFor<IApplicationSourceFinder>().Stub(x => x.FindSource(null, null))
                .Constraints(Is.Same(theSettings),
                             Is.TypeOf<ApplicationStartResponse>())
                .Return(null);


            theResponse = ClassUnderTest.StartApplication(theSettings);
        }

        [Test]
        public void the_status_should_denote_that_no_source_could_be_found()
        {
            theResponse.Status.ShouldEqual(ApplicationStartStatus.CouldNotResolveApplicationSource);
        }
    }

    [TestFixture]
    public class when_starting_an_application_runner_and_the_application_source_finding_fails : InteractionContext<ApplicationRunner>
    {
        private IApplicationSource theSource;
        private ApplicationSettings theSettings;
        private ApplicationStartResponse theResponse;
        private NotImplementedException theException;

        protected override void beforeEach()
        {
            theSettings = new ApplicationSettings();
            theSource = MockFor<IApplicationSource>();

            theException = new NotImplementedException();

            Services.PartialMockTheClassUnderTest();
            ClassUnderTest.Expect(x => x.StartApplication(theSource, theSettings))
                .Throw(theException);

            MockFor<IApplicationSourceFinder>().Stub(x => x.FindSource(null, null))
                .Constraints(Is.Same(theSettings),
                             Is.TypeOf<ApplicationStartResponse>())
                .Throw(theException);


            theResponse = ClassUnderTest.StartApplication(theSettings);
        }

        [Test]
        public void should_denote_application_source_failure()
        {
            theResponse.Status.ShouldEqual(ApplicationStartStatus.ApplicationSourceFailure);
        }

        [Test]
        public void the_response_should_have_the_error_message()
        {
            theResponse.ErrorMessage.ShouldEqual(theException.ToString());
        }
    }


    [TestFixture]
    public class when_starting_an_application_runner_and_the_application_fails : InteractionContext<ApplicationRunner>
    {
        private IApplicationSource theSource;
        private ApplicationSettings theSettings;
        private ApplicationStartResponse theResponse;
        private NotImplementedException theException;

        protected override void beforeEach()
        {
            theSettings = new ApplicationSettings();
            theSource = MockFor<IApplicationSource>();

            theException = new NotImplementedException();

            Services.PartialMockTheClassUnderTest();
            ClassUnderTest.Expect(x => x.StartApplication(theSource, theSettings))
                .Throw(theException);

            MockFor<IApplicationSourceFinder>().Stub(x => x.FindSource(null, null))
                .Constraints(Is.Same(theSettings),
                             Is.TypeOf<ApplicationStartResponse>())
                .Return(theSource);


            theResponse = ClassUnderTest.StartApplication(theSettings);
        }

        [Test]
        public void should_denote_application_source_failure()
        {
            theResponse.Status.ShouldEqual(ApplicationStartStatus.ApplicationSourceFailure);
        }

        [Test]
        public void the_response_should_have_the_error_message()
        {
            theResponse.ErrorMessage.ShouldEqual(theException.ToString());
        }
    }


    [TestFixture]
    public class when_starting_an_application_runner_successfully : InteractionContext<ApplicationRunner>
    {
        private IApplicationSource theSource;
        private ApplicationSettings theSettings;
        private ApplicationStartResponse theResponse;

        protected override void beforeEach()
        {
            theSettings = new ApplicationSettings();
            theSource = MockFor<IApplicationSource>();

            Services.PartialMockTheClassUnderTest();
            ClassUnderTest.Expect(x => x.StartApplication(theSource, theSettings));

            MockFor<IApplicationSourceFinder>().Stub(x => x.FindSource(null, null))
                .Constraints(Is.Same(theSettings),
                             Is.TypeOf<ApplicationStartResponse>())
                .Return(theSource);


            theResponse = ClassUnderTest.StartApplication(theSettings);
        }

        [Test]
        public void should_have_called_thru_to_start_the_application()
        {
            ClassUnderTest.VerifyAllExpectations();
        }

        [Test]
        public void the_response_should_denote_success()
        {
            theResponse.Status.ShouldEqual(ApplicationStartStatus.Started);
        }
    }


    public class Source1 : IApplicationSource
    {
        public FubuApplication BuildApplication()
        {
            throw new NotImplementedException();
        }
    }

    public class Source2 : Source1
    {
    }

    public class Source3 : Source1
    {
    }

    public class Source4 : Source1
    {
    }

    public class Source5 : Source1
    {
    }
}