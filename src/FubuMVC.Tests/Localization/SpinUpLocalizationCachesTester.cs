using FubuMVC.Core.Diagnostics.Packaging;
using FubuMVC.Core.Localization;
using FubuMVC.Core.Localization.Basic;
using FubuMVC.Tests.TestSupport;
using Xunit;
using Rhino.Mocks;
using Is = Rhino.Mocks.Constraints.Is;

namespace FubuMVC.Tests.Localization
{
    
    public class SpinUpLocalizationCachesTester : InteractionContext<SpinUpLocalizationCaches>
    {
        protected override void beforeEach()
        {
            ClassUnderTest.Activate(MockFor<IActivationLog>(), null);
        }

        [Fact]
        public void should_load_the_caches()
        {
            MockFor<ILocalizationProviderFactory>()
                .AssertWasCalled(x => x.LoadAll(null), x => x.Constraints(Is.NotNull()));
        }

        [Fact]
        public void should_apply_the_factory_to_localization_manager()
        {
            MockFor<ILocalizationProviderFactory>().AssertWasCalled(x => x.ApplyToLocalizationManager());
        }
    }
}