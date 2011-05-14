using FubuMVC.Core.View.Activation;
using FubuMVC.Spark.Rendering;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using Spark;

namespace FubuMVC.Spark.Tests.Rendering
{
    [TestFixture]
    public class PageActivationTester : InteractionContext<PageActivation>
    {
        private IPageActivator _activator;
        private IFubuSparkView _sparkView;
        private FubuSparkView _fubuSparkView;

        protected override void beforeEach()
        {
            _activator = MockFor<IPageActivator>();
            _sparkView = MockFor<IFubuSparkView>();
            _fubuSparkView = MockFor<FubuSparkView>();

            _activator.Expect(x => x.Activate(_fubuSparkView));
        }

        [Test]
        public void if_view_is_not_ifubupage_returns_false()
        {
            ClassUnderTest.Applies(_sparkView).ShouldBeFalse();
        }

        [Test]
        public void if_view_is_ifubupage_returns_true()
        {
            ClassUnderTest.Applies(_fubuSparkView).ShouldBeTrue();
        }

        [Test]
        public void the_activator_modifies_view_using_injected_activator()
        {
            ClassUnderTest.Modify(_fubuSparkView);
            _fubuSparkView.VerifyAllExpectations();
        }
    }
}