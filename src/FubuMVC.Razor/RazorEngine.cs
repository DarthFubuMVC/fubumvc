using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using Bottles;
using Bottles.Diagnostics;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Razor.RazorEngine;
using FubuMVC.Razor.Rendering;
using FubuMVC.Razor.RazorModel;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using IActivator = Bottles.IActivator;

namespace FubuMVC.Razor
{
    public class RazorEngineRegistry : IFubuRegistryExtension
    {
        private static bool _hasScanned;
        private static readonly TemplateRegistry _templateRegistry = new TemplateRegistry();
        private static readonly Lazy<TypePool> _types = new Lazy<TypePool>(getTypes);

        private ITemplateFinder _finder;
        private ITemplateComposer _composer;
        private readonly IPackageLog _logger;
        private readonly IList<ITemplateFinderConvention> _finderConventions = new List<ITemplateFinderConvention>();
        private readonly IList<ITemplateComposerConvention> _composerConventions = new List<ITemplateComposerConvention>();

        public RazorEngineRegistry()
        {
            _logger = getLogger();
            _finder = new TemplateFinder();
            _composer = new TemplateComposer(_types.Value);

            setupFinderDefaults();
            setupComposerDefaults();
        }

        private void setupFinderDefaults()
        {
            _finderConventions.Fill(new DefaultTemplateFinderConventions());
        }

        private void setupComposerDefaults()
        {
            _composerConventions.Apply(
                composer => composer
                    .AddBinder<ViewDescriptorBinder>()
                    .AddBinder<MasterPageBinder>()
                    .AddBinder<GenericViewModelBinder>()
                    .AddBinder<ViewModelBinder>()
                    .AddBinder<ViewLoaderBinder>()
                    .Apply<NamespacePolicy>()
                    .Apply<ViewPathPolicy>());
        }

        void IFubuRegistryExtension.Configure(FubuRegistry registry)
        {
            if (shouldScan())
            {
                scan(registry);
            }

            _logger.Trace("Adding [{0}] to registry [{1}]", typeof(RazorViewFacility).Name, registry.Name);
            registry.Views.Facility(new RazorViewFacility(_templateRegistry));
            registry.Services(configureServices);
        }

        private static bool shouldScan()
        {
            return !_hasScanned;
        }

        private void scan(IFubuRegistry parentRegistry)
        {
            var msg = "Initializing Razor in: [{0}]".ToFormat(parentRegistry.Name);
            _logger.Trace(ConsoleColor.Green, msg);

            findTemplates();
            composeTemplates();

            _hasScanned = true;
        }

        private void findTemplates()
        {
            var finder = _finder as TemplateFinder;
            if (finder != null)
            {
                _finderConventions.Each(t => t.Configure(finder));
            }

            _templateRegistry.Clear();
            _templateRegistry.AddRange(_finder.FindInHost());
            _templateRegistry.AddRange(_finder.FindInPackages());
        }

        private void composeTemplates()
        {
            var composer = _composer as TemplateComposer;
            if (composer != null)
            {
                _composerConventions.Each(c => c.Configure(composer));
            }

            _composer.Compose(_templateRegistry);
        }

        private static TypePool getTypes()
        {
            var types = new TypePool(FubuRegistry.FindTheCallingAssembly());

            var filter = new CompositeFilter<Assembly>();

            filter.Excludes.Add(a => a.IsDynamic);
            filter.Excludes.Add(a => types.HasAssembly(a));
            filter.Includes += (t => true);

            types.AddSource(() => AppDomain.CurrentDomain.GetAssemblies().Where(filter.MatchesAll));

            return types;
        }

        // TODO: Tests that ensure all of these defaults are set.

        private static void configureServices(IServiceRegistry services)
        {
            var configuration = new TemplateServiceConfiguration();
            configuration.BaseTemplateType = typeof(FubuRazorView);
            services.SetServiceIfNone<ITemplateRegistry>(_templateRegistry);
            services.AddService<ITemplateServiceWrapper>(new TemplateServiceWrapper(new TemplateService(configuration)));
            services.SetServiceIfNone<ITemplateServiceConfiguration>(configuration);

            services.FillType<IActivator, RazorActivator>();

            services.FillType<IRenderStrategy, NestedRenderStrategy>();
            services.FillType<IRenderStrategy, AjaxRenderStrategy>();
            services.FillType<IRenderStrategy, DefaultRenderStrategy>();

            services.SetServiceIfNone<IRazorViewEntryFactory, RazorViewEntryFactory>();
            services.SetServiceIfNone<IViewEntryProviderCache, ViewEntryProviderCache>();
            services.SetServiceIfNone<IViewModifierService, ViewModifierService>();

            services.FillType<IViewModifier, PageActivation>();
            services.FillType<IViewModifier, SiteResourceAttacher>();

            services.SetServiceIfNone<IHtmlEncoder, DefaultHtmlEncoder>();
        }

        private IPackageLog getLogger()
        {
            return PackageRegistry.Diagnostics != null
                ? PackageRegistry.Diagnostics.LogFor(this)
                : new PackageLog();
        }

        public RazorEngineRegistry FindWith(ITemplateFinder finder)
        {
            _finder = finder;
            return this;
        }

        public RazorEngineRegistry ConfigureFinder(ITemplateFinderConvention convention)
        {
            _finderConventions.Fill(convention);
            return this;
        }

        public RazorEngineRegistry ComposeWith(ITemplateComposer composer)
        {
            _composer = composer;
            return this;
        }

        public RazorEngineRegistry ConfigureComposer(ITemplateComposerConvention convention)
        {
            _composerConventions.Fill(convention);
            return this;
        }
    }

    public static class RazorExtensions
    {
        public static IList<ITemplateFinderConvention> Apply(this IList<ITemplateFinderConvention> source, Action<TemplateFinder> configure)
        {
            source.Fill(new LambdaTemplateFinderConvention(configure));
            return source;
        }

        public static IList<ITemplateComposerConvention> Apply(this IList<ITemplateComposerConvention> source, Action<TemplateComposer> configure)
        {
            source.Fill(new LambdaTemplateComposerConvention(configure));
            return source;
        }

        public static RazorEngineRegistry ConfigureFinder<TConvention>(this RazorEngineRegistry Razor)
            where TConvention : ITemplateFinderConvention, new()
        {
            return Razor.ConfigureFinder(new TConvention());
        }

        public static RazorEngineRegistry ConfigureFinder(this RazorEngineRegistry Razor, Action<TemplateFinder> configure)
        {
            return Razor.ConfigureFinder(new LambdaTemplateFinderConvention(configure));
        }

        public static RazorEngineRegistry FindWith<TFinder>(this RazorEngineRegistry Razor)
            where TFinder : ITemplateFinder, new()
        {
            return Razor.FindWith(new TFinder());
        }

        public static RazorEngineRegistry ComposeWith<TComposer>(this RazorEngineRegistry Razor)
            where TComposer : ITemplateComposer, new()
        {
            return Razor.ComposeWith(new TComposer());
        }

        public static RazorEngineRegistry ConfigureComposer<TConvention>(this RazorEngineRegistry Razor)
            where TConvention : ITemplateComposerConvention, new()
        {
            return Razor.ConfigureComposer(new TConvention());
        }

        public static RazorEngineRegistry ConfigureComposer(this RazorEngineRegistry Razor, Action<TemplateComposer> configure)
        {
            return Razor.ConfigureComposer(new LambdaTemplateComposerConvention(configure));
        }
    }
}