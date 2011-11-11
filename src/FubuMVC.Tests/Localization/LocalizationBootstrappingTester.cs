using System;
using System.Collections.Generic;
using System.Globalization;
using Bottles;
using Bottles.Diagnostics;
using FubuLocalization;
using FubuLocalization.Basic;
using FubuMVC.Core;
using FubuMVC.Core.Localization;
using FubuMVC.Core.Registration;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;

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
            registry.Import<BasicLocalizationSupport>();

            graphWithBasicLocalizationAsIs = registry.BuildGraph();
        }

        [Test]
        public void registers_us_english_as_the_default_culture()
        {
            graphWithBasicLocalizationAsIs.Services.DefaultServiceFor(typeof (CultureInfo))
                .Value.ShouldEqual(new CultureInfo("en-US"));
        }

        [Test]
        public void register_a_non_default_culture_info()
        {
            var registry = new FubuRegistry();
            registry.Import<BasicLocalizationSupport>(x => x.DefaultCulture = new CultureInfo("en-CA"));

            registry.BuildGraph().Services.DefaultServiceFor(typeof (CultureInfo))
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

            list.ShouldContain(typeof(RegisterXmlDirectoryLocalizationStorage));
            list.ShouldContain(typeof(SpinUpLocalizationCaches));

            list.IndexOf(typeof (RegisterXmlDirectoryLocalizationStorage))
                .ShouldBeLessThan(list.IndexOf(typeof (SpinUpLocalizationCaches)));
        }

        [Test]
        public void override_the_storage_mechanism_loader()
        {
            var registry = new FubuRegistry();
            registry.Import<BasicLocalizationSupport>(x =>
            {
                x.LoadLocalizationWith<StubLocalizationActivator>();
            });
            var graph = registry.BuildGraph();


            var list = graph.Services.ServicesFor<IActivator>().Select(x => x.Type).ToList();

            list.ShouldNotContain(typeof(RegisterXmlDirectoryLocalizationStorage));
            list.ShouldContain(typeof(StubLocalizationActivator));
            list.ShouldContain(typeof(SpinUpLocalizationCaches));

            list.IndexOf(typeof(StubLocalizationActivator))
                .ShouldBeLessThan(list.IndexOf(typeof(SpinUpLocalizationCaches)));
        }

        [Test]
        public void override_the_storage_mechanism()
        {
            var registry = new FubuRegistry();
            registry.Import<BasicLocalizationSupport>(x =>
            {
                x.LocalizationStorageIs<InMemoryLocalizationStorage>();
            });
            var graph = registry.BuildGraph();


            var list = graph.Services.ServicesFor<IActivator>().Select(x => x.Type).ToList();

            list.ShouldNotContain(typeof(RegisterXmlDirectoryLocalizationStorage));
            list.ShouldContain(typeof(SpinUpLocalizationCaches));

            graph.Services.DefaultServiceFor<ILocalizationStorage>().Type.ShouldEqual(
                typeof (InMemoryLocalizationStorage));
        }

        public class StubLocalizationActivator : IActivator
        {
            public void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log)
            {
                throw new NotImplementedException();
            }
        }
    }
}