using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bottles;
using Bottles.Diagnostics;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.View.Conventions;
using FubuMVC.Core.View.Model;
using FubuMVC.Core.View.Model.Sharing;
using FubuMVC.Core.View.Rendering;
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
        private static readonly TemplateRegistry<IRazorTemplate> _templateRegistry = new TemplateRegistry<IRazorTemplate>();
        private static readonly Lazy<TypePool> _types = new Lazy<TypePool>(getTypes);
        private static readonly RazorParsings _parsings = new RazorParsings();

        private ITemplateFinder<Template> _finder;
        private ITemplateComposer<IRazorTemplate> _composer;
        private readonly IPackageLog _logger;
        private readonly IList<ITemplateFinderConvention<Template>> _finderConventions = new List<ITemplateFinderConvention<Template>>();
        private readonly IList<ITemplateComposerConvention<IRazorTemplate>> _composerConventions = new List<ITemplateComposerConvention<IRazorTemplate>>();

        public RazorEngineRegistry()
        {
            _logger = getLogger();
            _finder = new TemplateFinder<Template>();
            _composer = new TemplateComposer<IRazorTemplate>(_types.Value, _parsings);

            setupFinderDefaults();
            setupComposerDefaults();
        }

        private void setupFinderDefaults()
        {
            _finderConventions.Fill(new DefaultRazorTemplateFinderConventions());
        }

        private void setupComposerDefaults()
        {
            _composerConventions.Apply(
                composer => composer
                    .AddBinder(new ViewDescriptorBinder<IRazorTemplate>(new RazorTemplateSelector()))
                    .AddBinder<GenericViewModelBinder<IRazorTemplate>>()
                    .AddBinder<ViewModelBinder<IRazorTemplate>>()
                    .Apply<ViewPathPolicy<IRazorTemplate>>());
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
            parseTemplates();
            composeTemplates();

            _hasScanned = true;
        }

        private void parseTemplates()
        {
            _templateRegistry.Each(t => _parsings.Parse(t));
        }

        private void findTemplates()
        {
            var finder = _finder as TemplateFinder<Template>;
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
            var composer = _composer as TemplateComposer<IRazorTemplate>;
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

        private static void configureServices(IServiceRegistry services)
        {
            var configuration = new TemplateServiceConfiguration();
            configuration.BaseTemplateType = typeof(FubuRazorView);
            services.SetServiceIfNone<ITemplateRegistry<IRazorTemplate>>(_templateRegistry);
            services.AddService<IFubuTemplateService>(new FubuTemplateService(_templateRegistry, new TemplateService(configuration), new FileSystem()));
            services.SetServiceIfNone<ITemplateServiceConfiguration>(configuration);
            services.SetServiceIfNone<IParsingRegistrations<IRazorTemplate>>(_parsings);
            services.SetServiceIfNone<ITemplateDirectoryProvider<IRazorTemplate>, TemplateDirectoryProvider<IRazorTemplate>>();
            services.SetServiceIfNone<ISharedPathBuilder>(new SharedPathBuilder());

            var graph = new SharingGraph();
            services.SetServiceIfNone(graph);
            services.SetServiceIfNone<ISharingGraph>(graph);


            services.FillType<IActivator, RazorActivator>();

            services.FillType<ISharedTemplateLocator<IRazorTemplate>, SharedTemplateLocator<IRazorTemplate>>();
            services.FillType<ISharingAttacher<IRazorTemplate>, MasterAttacher<IRazorTemplate>>();
            services.FillType<ITemplateSelector<IRazorTemplate>, RazorTemplateSelector>();
            services.FillType<IActivator, SharingAttacherActivator<IRazorTemplate>>();
            services.FillType<IRenderStrategy, AjaxRenderStrategy>();
            services.FillType<IRenderStrategy, DefaultRenderStrategy>();

            services.SetServiceIfNone<IViewModifierService<IFubuRazorView>, ViewModifierService<IFubuRazorView>>();

            services.FillType<IViewModifier<IFubuRazorView>, PageActivation<IFubuRazorView>>();
            services.FillType<IViewModifier<IFubuRazorView>, LayoutActivation>();
            services.FillType<IViewModifier<IFubuRazorView>, PartialRendering>();
            services.FillType<IViewModifier<IFubuRazorView>, FubuPartialRendering>();
        }

        private IPackageLog getLogger()
        {
            return PackageRegistry.Diagnostics != null
                ? PackageRegistry.Diagnostics.LogFor(this)
                : new PackageLog();
        }

        public RazorEngineRegistry FindWith(ITemplateFinder<Template> finder)
        {
            _finder = finder;
            return this;
        }

        public RazorEngineRegistry ConfigureFinder(ITemplateFinderConvention<Template> convention)
        {
            _finderConventions.Fill(convention);
            return this;
        }

        public RazorEngineRegistry ComposeWith(ITemplateComposer<IRazorTemplate> composer)
        {
            _composer = composer;
            return this;
        }

        public RazorEngineRegistry ConfigureComposer(ITemplateComposerConvention<IRazorTemplate> convention)
        {
            _composerConventions.Fill(convention);
            return this;
        }
    }

    public static class RazorExtensions
    {
        public static IList<ITemplateFinderConvention<Template>> Apply(this IList<ITemplateFinderConvention<Template>> source, Action<TemplateFinder<Template>> configure)
        {
            source.Fill(new LambdaTemplateFinderConvention<Template>(configure));
            return source;
        }

        public static IList<ITemplateComposerConvention<IRazorTemplate>> Apply(this IList<ITemplateComposerConvention<IRazorTemplate>> source, Action<TemplateComposer<IRazorTemplate>> configure)
        {
            source.Fill(new LambdaTemplateComposerConvention<IRazorTemplate>(configure));
            return source;
        }

        public static RazorEngineRegistry ConfigureFinder<TConvention>(this RazorEngineRegistry razor)
            where TConvention : ITemplateFinderConvention<Template>, new()
        {
            return razor.ConfigureFinder(new TConvention());
        }

        public static RazorEngineRegistry ConfigureFinder(this RazorEngineRegistry razor, Action<TemplateFinder<Template>> configure)
        {
            return razor.ConfigureFinder(new LambdaTemplateFinderConvention<Template>(configure));
        }

        public static RazorEngineRegistry FindWith<TFinder>(this RazorEngineRegistry razor)
            where TFinder : ITemplateFinder<Template>, new()
        {
            return razor.FindWith(new TFinder());
        }

        public static RazorEngineRegistry ComposeWith<TComposer>(this RazorEngineRegistry razor)
            where TComposer : ITemplateComposer<IRazorTemplate>, new()
        {
            return razor.ComposeWith(new TComposer());
        }

        public static RazorEngineRegistry ConfigureComposer<TConvention>(this RazorEngineRegistry razor)
            where TConvention : ITemplateComposerConvention<IRazorTemplate>, new()
        {
            return razor.ConfigureComposer(new TConvention());
        }

        public static RazorEngineRegistry ConfigureComposer(this RazorEngineRegistry razor, Action<TemplateComposer<IRazorTemplate>> configure)
        {
            return razor.ConfigureComposer(new LambdaTemplateComposerConvention<IRazorTemplate>(configure));
        }
    }
}