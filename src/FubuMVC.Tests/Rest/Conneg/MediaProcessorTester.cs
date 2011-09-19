using FubuMVC.Core;
using FubuMVC.Core.Rest.Conneg;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Rest.Conneg
{
    [TestFixture]
    public class MediaProcessorTester : InteractionContext<MediaProcessor<MediaProcessorInput>>
    {
        private IFormatter[] theFormatters;
        private CurrentRequest theCurrentRequest;

        protected override void beforeEach()
        {
            theFormatters = Services.CreateMockArrayFor<IFormatter>(3);
            theCurrentRequest = new CurrentRequest();
        }

        [Test]
        public void retrieving_exercises_the_first_matching_formatter()
        {
            theFormatters[0].Stub(x => x.Matches(theCurrentRequest)).Return(true);
            var theExpectedValue = new MediaProcessorInput();
            theFormatters[0].Stub(x => x.Read<MediaProcessorInput>(theCurrentRequest)).Return(theExpectedValue);

            ClassUnderTest.Retrieve(theCurrentRequest).ShouldBeTheSameAs(theExpectedValue);
        }

        [Test]
        public void retrieving_exercises_the_first_matching_formatter_2()
        {
            theFormatters[2].Stub(x => x.Matches(theCurrentRequest)).Return(true);
            var theExpectedValue = new MediaProcessorInput();
            theFormatters[2].Stub(x => x.Read<MediaProcessorInput>(theCurrentRequest)).Return(theExpectedValue);

            ClassUnderTest.Retrieve(theCurrentRequest).ShouldBeTheSameAs(theExpectedValue);
        }

        [Test]
        public void write_exercises_the_first_matching_formatter()
        {
            var theExpectedValue = new MediaProcessorInput();
            theFormatters[0].Stub(x => x.Matches(theCurrentRequest)).Return(true);

            ClassUnderTest.Write(theExpectedValue, theCurrentRequest);

            theFormatters[0].AssertWasCalled(x => x.Write(theExpectedValue, theCurrentRequest));
            theFormatters[1].AssertWasNotCalled(x => x.Write(theExpectedValue, theCurrentRequest));
            theFormatters[2].AssertWasNotCalled(x => x.Write(theExpectedValue, theCurrentRequest));
        }


        [Test]
        public void write_exercises_the_first_matching_formatter_2()
        {
            var theExpectedValue = new MediaProcessorInput();
            theFormatters[2].Stub(x => x.Matches(theCurrentRequest)).Return(true);

            ClassUnderTest.Write(theExpectedValue, theCurrentRequest);

            theFormatters[0].AssertWasNotCalled(x => x.Write(theExpectedValue, theCurrentRequest));
            theFormatters[1].AssertWasNotCalled(x => x.Write(theExpectedValue, theCurrentRequest));
            theFormatters[2].AssertWasCalled(x => x.Write(theExpectedValue, theCurrentRequest));
        }
    }

    public class MediaProcessorInput
    {
        
    }
}