using FubuMVC.Core.Http;
using FubuMVC.Core.Rest.Media;
using FubuMVC.Core.Runtime;
using FubuMVC.Tests.UI;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Rest.Media
{

    // MediaChoice is very simple *now*, but will become more complex later
    // when it's time to optimize
    [TestFixture]
    public class MediaChoiceTester : InteractionContext<MediaChoice<Address>>
    {
        [Test]
        public void matches_delegates_to_the_internal_conditional()
        {
            MockFor<IRequestConditional>().Stub(x => x.Matches("text/json")).Return(true);

            ClassUnderTest.Matches("text/json").ShouldBeTrue();
            ClassUnderTest.Matches("text/xml").ShouldBeFalse();
        }

        [Test]
        public void write_delegates_to_the_media_writer()
        {
            var writer = MockFor<IOutputWriter>();
            var address = new Address();
            ClassUnderTest.Write(address);
        
            MockFor<IMediaWriter<Address>>().AssertWasCalled(x => x.Write(address, writer));
        }
    }
}