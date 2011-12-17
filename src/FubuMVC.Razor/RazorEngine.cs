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
using FubuMVC.Razor.Rendering;
using FubuMVC.Razor.RazorModel;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using IActivator = Bottles.IActivator;

namespace FubuMVC.Razor
{
    public class RazorEngine : IFubuRegistryExtension
    {
        private static bool _hasScanned;
        private static readonly TemplateRegistry _templateRegistry = new TemplateRegistry();
        private static readonly Lazy<TypePool> _types = new Lazy<TypePool>(getTypes);

        private ITemplateFinder _finder;
        private ITemplateComposer _composer;
        private readonly IPackageLog _logger;
        private readonly IList<ITemplateFinderConvention> _finderConventions = new List<ITemplateFinderConvention>();
        private readonly IList<ITemplateComposerConvention> _composerConventions = new List<ITemplateComposerConvention>();

        public RazorEngine()
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
                    .AddBinder<ReachableBindingsBinder>()
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
            if(composer != null)
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
            services.SetServiceIfNone<ITemplateRegistry>(_templateRegistry);
            services.SetServiceIfNone<ITemplateService>(new TemplateService(configuration));
            services.SetServiceIfNone<ITemplateServiceConfiguration>(configuration);

            services.FillType<IActivator, RazorActivator>();

            services.FillType<IRenderStrategy, NestedRenderStrategy>();
            services.FillType<IRenderStrategy, AjaxRenderStrategy>();
            services.FillType<IRenderStrategy, DefaultRenderStrategy>();

            services.SetServiceIfNone<IViewEntryProviderCache, ViewEntryProviderCache>();
            services.SetServiceIfNone<IViewModifierService, ViewModifierService>();

            services.FillType<IViewModifier, PageActivation>();
            services.FillType<IViewModifier, SiteResourceAttacher>();

            services.SetServiceIfNone<IHtmlEncoder, DefaultHtmlEncoder>();

            services.SetServiceIfNone(new DefaultViewDefinitionPolicy());
            services.SetServiceIfNone<IViewDefinitionResolver, ViewDefinitionResolver>();
        }

        private IPackageLog getLogger()
        {
            return PackageRegistry.Diagnostics != null
                ? PackageRegistry.Diagnostics.LogFor(this)
                : new PackageLog();
        }

        public RazorEngine FindWith(ITemplateFinder finder)
        {
            _finder = finder;
            return this;
        }

        public RazorEngine ConfigureFinder(ITemplateFinderConvention convention)
        {
            _finderConventions.Fill(convention);
            return this;
        }

        public RazorEngine ComposeWith(ITemplateComposer composer)
        {
            _composer = composer;
            return this;
        }

        public RazorEngine ConfigureComposer(ITemplateComposerConvention convention)
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

        public static RazorEngine ConfigureFinder<TConvention>(this RazorEngine Razor)
            where TConvention : ITemplateFinderConvention, new()
        {
            return Razor.ConfigureFinder(new TConvention());
        }

        public static RazorEngine ConfigureFinder(this RazorEngine Razor, Action<TemplateFinder> configure)
        {
            return Razor.ConfigureFinder(new LambdaTemplateFinderConvention(configure));
        }

        public static RazorEngine FindWith<TFinder>(this RazorEngine Razor)
            where TFinder : ITemplateFinder, new()
        {
            return Razor.FindWith(new TFinder());
        }

        public static RazorEngine ComposeWith<TComposer>(this RazorEngine Razor)
            where TComposer : ITemplateComposer, new()
        {
            return Razor.ComposeWith(new TComposer());
        }

        public static RazorEngine ConfigureComposer<TConvention>(this RazorEngine Razor)
            where TConvention : ITemplateComposerConvention, new()
        {
            return Razor.ConfigureComposer(new TConvention());
        }

        public static RazorEngine ConfigureComposer(this RazorEngine Razor, Action<TemplateComposer> configure)
        {
            return Razor.ConfigureComposer(new LambdaTemplateComposerConvention(configure));
        }
    }
}