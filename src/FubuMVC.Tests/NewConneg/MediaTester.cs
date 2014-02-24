using FubuMVC.Core;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Conditionals;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.NewConneg
{
    [TestFixture]
    public class MediaTester : InteractionContext<Media<OutputTarget>>
    {
        [Test]
        public void matches_request_positive()
        {
            var context = MockRepository.GenerateMock<IFubuRequestContext>();

            MockFor<IConditional>().Stub(x => x.ShouldExecute(context)).Return(true);

            ClassUnderTest.MatchesRequest(context).ShouldBeTrue();
        }

        [Test]
        public void matches_request_negative()
        {
            var context = MockRepository.GenerateMock<IFubuRequestContext>();
            MockFor<IConditional>().Stub(x => x.ShouldExecute(context)).Return(false);

            ClassUnderTest.MatchesRequest(context).ShouldBeFalse();
        }

        [Test]
        public void write_will_delegate_to_the_inner_writer()
        {
            var context = MockRepository.GenerateMock<IFubuRequestContext>();

            var theTarget = new OutputTarget();
            ClassUnderTest.Write(MimeType.Html.Value, context, theTarget);
        
            MockFor<IMediaWriter<OutputTarget>>()
                .AssertWasCalled(x => x.Write(MimeType.Html.Value, context, theTarget));
        }

        [Test]
        public void mime_types_just_delegates()
        {
            MockFor<IMediaWriter<OutputTarget>>()
                .Stub(x => x.Mimetypes)
                .Return(new[]{MimeType.Html.Value, MimeType.Json.Value});

            ClassUnderTest.Mimetypes.ShouldHaveTheSameElementsAs(MimeType.Html.Value, MimeType.Json.Value);
        }
    }
}