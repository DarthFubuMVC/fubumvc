using FubuMVC.Core.View.Activation;
using FubuMVC.Core.View.Rendering;
using FubuMVC.Razor.Rendering;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Razor.Tests.Rendering
{
    [TestFixture]
    public class PageActivationTester : InteractionContext<PageActivation<IFubuRazorView>>
    {
        private IPageActivator _activator;
        private IFubuRazorView _razorView;

        protected override void beforeEach()
        {
            _activator = MockFor<IPageActivator>();
            _razorView = MockFor<IFubuRazorView>();

            _activator.Expect(x => x.Activate(_razorView));
        }

        [Test]
        public void it_applies_for_all_the_views()
        {
            ClassUnderTest.Applies(_razorView).ShouldBeTrue();
        }

        [Test]
        public void the_activator_modifies_view_using_injected_activator()
        {
            ClassUnderTest.Modify(_razorView);
            _razorView.VerifyAllExpectations();
        }
    }
}