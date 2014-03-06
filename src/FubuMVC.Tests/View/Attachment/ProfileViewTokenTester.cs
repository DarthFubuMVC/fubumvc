using FubuMVC.Core.View;
using FubuMVC.Core.View.Attachment;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.View.Attachment
{
    [TestFixture]
    public class ProfileViewTokenTester
    {
        private IViewToken theInner;
        private ProfileViewToken theToken;

        [SetUp]
        public void SetUp()
        {
            theInner = MockRepository.GenerateMock<IViewToken>();
            theToken = new ProfileViewToken(theInner, "filtered name");
        }

        [Test]
        public void the_name_is_what_was_passed_in()
        {
            theInner.Stub(x => x.Name()).Throw(new AssertionException("Don't call me"));

            theToken.Name().ShouldEqual("filtered name");
        }

        [Test]
        public void view_model_delegates()
        {
            theInner.Stub(x => x.ViewModel).Return(typeof (string));
            theToken.ViewModel.ShouldEqual(typeof (string));
        }

        [Test]
        public void name_space_delegates()
        {
            theInner.Stub(x => x.Namespace).Return("something.else");
            theToken.Namespace.ShouldEqual("something.else");
        }

    }
}