using FubuMVC.Core.View.Attachment;
using NUnit.Framework;
using Rhino.Mocks;
using FubuTestingSupport;

namespace FubuMVC.Core.View.Testing.Attachment
{
    [TestFixture]
    public class ViewTokenExtensionsTester
    {
        [Test]
        public void if_a_token_is_ProfileViewToken_it_resolves_to_the_inner_token()
        {
            var theInner = MockRepository.GenerateMock<IViewToken>();
            var theToken = new ProfileViewToken(theInner, "filtered name");

            theToken.Resolve().ShouldBeTheSameAs(theInner);
        }

        [Test]
        public void if_a_token_is_not_profile_view_token_it_resolves_to_itself()
        {
            var theInner = MockRepository.GenerateMock<IViewToken>();
            theInner.Resolve().ShouldBeTheSameAs(theInner);
        }
    }
}