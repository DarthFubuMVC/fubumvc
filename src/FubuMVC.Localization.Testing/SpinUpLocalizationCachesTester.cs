using Bottles.Diagnostics;
using FubuLocalization.Basic;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using Is = Rhino.Mocks.Constraints.Is;

namespace FubuMVC.Localization.Testing
{
    [TestFixture]
    public class SpinUpLocalizationCachesTester : InteractionContext<SpinUpLocalizationCaches>
    {
        protected override void beforeEach()
        {
            ClassUnderTest.Activate(MockFor<IPackageLog>());
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