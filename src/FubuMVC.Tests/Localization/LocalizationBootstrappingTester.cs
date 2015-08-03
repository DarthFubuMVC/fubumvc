using System.Globalization;
using FubuLocalization;
using FubuLocalization.Basic;
using FubuMVC.Core;
using FubuMVC.Core.Localization;
using FubuMVC.Core.Registration;
using NUnit.Framework;
using Shouldly;
using StructureMap;

namespace FubuMVC.Tests.Localization
{
    [TestFixture]
    public class LocalizationBootstrappingTester
    {
        [Test]
        public void service_registrations()
        {
            var registry = new FubuRegistry();
            registry.Features.Localization.Enable(true);

            using (var runtime = registry.ToRuntime())
            {
                var container = runtime.Get<IContainer>();

                container.GetInstance<CultureInfo>().Name.ShouldBe("en-US");
                container.DefaultRegistrationIs<ICurrentCultureContext, CurrentCultureContext>();
                container.DefaultRegistrationIs<ILocalizationMissingHandler, LocalizationMissingHandler>();
                container.DefaultRegistrationIs<ILocalizationProviderFactory, LocalizationProviderFactory>();
                container.DefaultSingletonIs<ILocalizationCache, LocalizationCache>();

                container.ShouldHaveRegistration<IActivator, SpinUpLocalizationCaches>();
            }
        }


        private BehaviorGraph graphWithBasicLocalizationAsIs;

        [SetUp]
        public void SetUp()
        {
            var registry = new FubuRegistry();
            registry.Features.Localization.Enable(true);

            graphWithBasicLocalizationAsIs = BehaviorGraph.BuildFrom(registry);
        }


        [Test]
        public void register_a_non_default_culture_info()
        {
            var registry = new FubuRegistry();
            registry.Features.Localization.Enable(true);
            registry.Features.Localization.Configure(x => { x.DefaultCulture = new CultureInfo("en-CA"); });

            using (var runtime = registry.ToRuntime())
            {
                runtime.Get<CultureInfo>().Name.ShouldBe("en-CA");
            }
        }
    }
}