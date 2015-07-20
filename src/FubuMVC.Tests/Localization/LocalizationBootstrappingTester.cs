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
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Localization
{
    [TestFixture]
    public class LocalizationBootstrappingTester
    {
        private BehaviorGraph graphWithBasicLocalizationAsIs;

        [SetUp]
        public void SetUp()
        {
            var registry = new FubuRegistry();
            registry.Features.Localization.Enable(true);

            graphWithBasicLocalizationAsIs = BehaviorGraph.BuildFrom(registry);
        }

        [Test]
        public void registers_us_english_as_the_default_culture()
        {
            graphWithBasicLocalizationAsIs.Services.DefaultServiceFor(typeof (CultureInfo))
                .Value.ShouldEqual(new CultureInfo("en-US"));
        }

        [Test]
        public void registers_us_english_as_the_default_culture_context()
        {
            var context = graphWithBasicLocalizationAsIs.Services.DefaultServiceFor(typeof(ICurrentCultureContext)).Value.As<CurrentCultureContext>();
            context.CurrentCulture.ShouldEqual(new CultureInfo("en-US"));
            context.CurrentUICulture.ShouldEqual(new CultureInfo("en-US"));
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

            BehaviorGraph.BuildFrom(registry).Services.DefaultServiceFor(typeof (CultureInfo))
                .Value.ShouldEqual(new CultureInfo("en-CA"));
        }

        [Test]
        public void localization_cache_is_registered()
        {
            graphWithBasicLocalizationAsIs.Services.DefaultServiceFor<ILocalizationCache>()
                .Type.ShouldEqual(typeof (LocalizationCache));
        }

        [Test]
        public void localization_missing_handler_is_registered()
        {
            graphWithBasicLocalizationAsIs.Services.DefaultServiceFor<ILocalizationMissingHandler>()
                .Type.ShouldEqual(typeof(LocalizationMissingHandler));
        }

        [Test]
        public void localization_provider_registry_is_registered()
        {
            graphWithBasicLocalizationAsIs.Services.DefaultServiceFor<ILocalizationProviderFactory>()
                .Type.ShouldEqual(typeof(LocalizationProviderFactory));
        }

        [Test]
        public void if_using_the_basic_storage_registers_both_activators()
        {
            var list = graphWithBasicLocalizationAsIs.Services.ServicesFor<IActivator>().Select(x => x.Type).ToList();

            list.ShouldContain(typeof(SpinUpLocalizationCaches));
        }

        public class StubLocalizationActivator : IActivator
        {
            public void Activate(IActivationLog log)
            {
                throw new NotImplementedException();
            }
        }
    }
}
