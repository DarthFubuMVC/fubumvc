using FubuMVC.Razor.Rendering;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Razor.Tests.Rendering
{
    [TestFixture]
    public class FubuPartialRenderingTester : InteractionContext<FubuPartialRendering>
    {
        private IFubuRazorView _view;

        protected override void beforeEach()
        {
            _view = MockFor<IFubuRazorView>();
        }

        [Test]
        public void when_first_executing_applies_returns_false()
        {
            ClassUnderTest.Applies(_view).ShouldBeFalse();
        }

        [Test]
        public void after_first_applies_call_applies_always_returns_true()
        {
            ClassUnderTest.Applies(_view).ShouldBeFalse();
            ClassUnderTest.Applies(_view).ShouldBeTrue();
            ClassUnderTest.Applies(_view).ShouldBeTrue();
            ClassUnderTest.Applies(_view).ShouldBeTrue();
        }
    }
}