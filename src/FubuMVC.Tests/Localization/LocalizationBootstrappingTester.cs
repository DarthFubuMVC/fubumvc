using System;
using System.Globalization;
using System.Linq;
using FubuCore;
using FubuLocalization;
using FubuLocalization.Basic;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics.Packaging;
using FubuMVC.Core.Localization;
using FubuMVC.Core.Registration;
using Shouldly;
using NUnit.Framework;

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
                runtime.Container.GetInstance<CultureInfo>().Name.ShouldBe("en-US");
                runtime.Container.DefaultRegistrationIs<ICurrentCultureContext, CurrentCultureContext>();
                runtime.Container.DefaultRegistrationIs<ILocalizationMissingHandler, LocalizationMissingHandler>();
                runtime.Container.DefaultRegistrationIs<ILocalizationProviderFactory, LocalizationProviderFactory>();
                runtime.Container.DefaultSingletonIs<ILocalizationCache, LocalizationCache>();

                runtime.Container.ShouldHaveRegistration<IActivator, SpinUpLocalizationCaches>();
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
            registry.Features.Localization.Configure(x =>
            {
                x.DefaultCulture = new CultureInfo("en-CA");
            });

            using (var runtime = registry.ToRuntime())
            {
                runtime.Container.GetInstance<CultureInfo>().Name.ShouldBe("en-CA");
            }
        }



    }
}
