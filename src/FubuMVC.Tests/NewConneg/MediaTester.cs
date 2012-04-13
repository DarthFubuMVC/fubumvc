using FubuMVC.Core.Behaviors.Conditional;
using FubuMVC.Core.Resources.Conneg.New;
using FubuMVC.Core.Runtime;
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
            MockFor<IConditional>().Stub(x => x.ShouldExecute()).Return(true);

            ClassUnderTest.MatchesRequest().ShouldBeTrue();
        }

        [Test]
        public void matches_request_negative()
        {
            MockFor<IConditional>().Stub(x => x.ShouldExecute()).Return(false);

            ClassUnderTest.MatchesRequest().ShouldBeFalse();
        }

        [Test]
        public void write_will_delegate_to_the_inner_writer()
        {
            var theTarget = new OutputTarget();
            ClassUnderTest.Write(MimeType.Html.Value, theTarget);
        
            MockFor<IMediaWriter<OutputTarget>>()
                .AssertWasCalled(x => x.Write(MimeType.Html.Value, theTarget));
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