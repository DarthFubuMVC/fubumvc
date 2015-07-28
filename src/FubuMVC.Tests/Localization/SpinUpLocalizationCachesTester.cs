using FubuLocalization.Basic;
using FubuMVC.Core.Diagnostics.Packaging;
using FubuMVC.Core.Localization;
using FubuMVC.Tests.TestSupport;
using NUnit.Framework;
using Rhino.Mocks;
using Is = Rhino.Mocks.Constraints.Is;

namespace FubuMVC.Tests.Localization
{
    [TestFixture]
    public class SpinUpLocalizationCachesTester : InteractionContext<SpinUpLocalizationCaches>
    {
        protected override void beforeEach()
        {
            ClassUnderTest.Activate(MockFor<IActivationLog>(), null);
        }

        [Test]
        public void should_load_the_caches()
        {
            MockFor<ILocalizationProviderFactory>()
                .AssertWasCalled(x => x.LoadAll(null), x => x.Constraints(Is.NotNull()));
        }

        [Test]
        public void should_apply_the_factory_to_localization_manager()
        {
            MockFor<ILocalizationProviderFactory>().AssertWasCalled(x => x.ApplyToLocalizationManager());
        }
    }
}