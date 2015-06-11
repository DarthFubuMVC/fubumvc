using Bottles;
using Bottles.Diagnostics;
using FubuLocalization.Basic;
using FubuTestingSupport;
using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using Is = Rhino.Mocks.Constraints.Is;

namespace FubuMVC.Localization.Testing
{
    [TestFixture]
    public class SpinUpLocalizationCachesTester : InteractionContext<SpinUpLocalizationCaches>
    {
        private List<IPackageInfo> thePackages;

        protected override void beforeEach()
        {
            thePackages = new List<IPackageInfo>();
            ClassUnderTest.Activate(thePackages, MockFor<IPackageLog>());
        }

        [Test]
        public void should_load_the_caches()
        {
            MockFor<ILocalizationProviderFactory>().AssertWasCalled(x => x.LoadAll(null), x => x.Constraints(Is.NotNull()));
        }

        [Test]
        public void should_apply_the_factory_to_localization_manager()
        {
            MockFor<ILocalizationProviderFactory>().AssertWasCalled(x => x.ApplyToLocalizationManager());
        }
    }
}