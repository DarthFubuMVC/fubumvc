using System;
using System.Collections.Generic;
using System.Globalization;
using Bottles;
using Bottles.Diagnostics;
using FubuLocalization;
using FubuLocalization.Basic;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using NUnit.Framework;
using System.Linq;
using FubuCore;
using FubuTestingSupport;

namespace FubuMVC.Localization.Testing
{
    [TestFixture]
    public class LocalizationBootstrappingTester
    {
        private BehaviorGraph graphWithBasicLocalizationAsIs;

        [SetUp]
        public void SetUp()
        {
            var registry = new FubuRegistry();
            registry.Import<BasicLocalizationSupport>();

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
            registry.Import<BasicLocalizationSupport>(x => x.DefaultCulture = new CultureInfo("en-CA"));

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
            public void Activate(IPackageLog log)
            {
                throw new NotImplementedException();
            }
        }
    }
}
