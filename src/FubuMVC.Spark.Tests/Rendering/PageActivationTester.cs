using FubuMVC.Core.View.Activation;
using FubuMVC.Core.View.Rendering;
using FubuMVC.Spark.Rendering;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Spark.Tests.Rendering
{
    [TestFixture]
    public class PageActivationTester : InteractionContext<PageActivation<IFubuSparkView>>
    {
        private IPageActivator _activator;
        private IFubuSparkView _sparkView;

        protected override void beforeEach()
        {
            _activator = MockFor<IPageActivator>();
            _sparkView = MockFor<IFubuSparkView>();
            MockFor<FubuSparkView>();

            _activator.Expect(x => x.Activate(_sparkView));
        }

        [Test]
        public void it_applies_for_all_the_views()
        {
            ClassUnderTest.Applies(_sparkView).ShouldBeTrue();
        }

        [Test]
        public void the_activator_modifies_view_using_injected_activator()
        {
            ClassUnderTest.Modify(_sparkView);
            _sparkView.VerifyAllExpectations();
        }
    }
}