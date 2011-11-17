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
using FubuMVC.Spark.Rendering;
using FubuMVC.Spark.SparkModel;
using Spark;
using Spark.Caching;

namespace FubuMVC.Spark
{
    public class SparkEngine : IFubuRegistryExtension
    {
        private static bool _hasScanned;
        private static readonly TemplateRegistry _templateRegistry = new TemplateRegistry();
        private static readonly Lazy<TypePool> _types = new Lazy<TypePool>(getTypes);

        private ITemplateFinder _finder;
        private ITemplateComposer _composer;
        private readonly IPackageLog _logger;
        private readonly IList<ITemplateFinderConvention> _finderConventions = new List<ITemplateFinderConvention>();
        private readonly IList<ITemplateComposerConvention> _composerConventions = new List<ITemplateComposerConvention>();

        public SparkEngine()
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

            _logger.Trace("Adding [{0}] to registry [{1}]", typeof(SparkViewFacility).Name, registry.Name);
            registry.Views.Facility(new SparkViewFacility(_templateRegistry));
            registry.Services(configureServices);
        }

        private static bool shouldScan()
        {
            return !_hasScanned;
        }

        private void scan(IFubuRegistry parentRegistry)
        {
            var msg = "Initializing Spark in: [{0}]".ToFormat(parentRegistry.Name);
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
            services.SetServiceIfNone<ITemplateRegistry>(_templateRegistry);
            services.SetServiceIfNone<ISparkViewEngine>(new SparkViewEngine());
            services.SetServiceIfNone<ICacheService>(new DefaultCacheService(HttpRuntime.Cache));

            services.FillType<IActivator, SparkActivator>();

            services.FillType<IRenderStrategy, NestedRenderStrategy>();
            services.FillType<IRenderStrategy, AjaxRenderStrategy>();
            services.FillType<IRenderStrategy, DefaultRenderStrategy>();

            services.SetServiceIfNone<IViewEntryProviderCache, ViewEntryProviderCache>();
            services.SetServiceIfNone<IViewModifierService, ViewModifierService>();

            services.FillType<IViewModifier, PageActivation>();
            services.FillType<IViewModifier, SiteResourceAttacher>();
            services.FillType<IViewModifier, ContentActivation>();
            services.FillType<IViewModifier, OnceTableActivation>();
            services.FillType<IViewModifier, OuterViewOutputActivator>();
            services.FillType<IViewModifier, NestedViewOutputActivator>();
            services.FillType<IViewModifier, ViewContentDisposer>();
            services.FillType<IViewModifier, NestedOutputActivation>();

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

        public SparkEngine FindWith(ITemplateFinder finder)
        {
            _finder = finder;
            return this;
        }

        public SparkEngine ConfigureFinder(ITemplateFinderConvention convention)
        {
            _finderConventions.Fill(convention);
            return this;
        }

        public SparkEngine ComposeWith(ITemplateComposer composer)
        {
            _composer = composer;
            return this;
        }

        public SparkEngine ConfigureComposer(ITemplateComposerConvention convention)
        {
            _composerConventions.Fill(convention);
            return this;
        }
    }

    public static class SparkExtensions
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

        public static SparkEngine ConfigureFinder<TConvention>(this SparkEngine spark)
            where TConvention : ITemplateFinderConvention, new()
        {
            return spark.ConfigureFinder(new TConvention());
        }

        public static SparkEngine ConfigureFinder(this SparkEngine spark, Action<TemplateFinder> configure)
        {
            return spark.ConfigureFinder(new LambdaTemplateFinderConvention(configure));
        }

        public static SparkEngine FindWith<TFinder>(this SparkEngine spark)
            where TFinder : ITemplateFinder, new()
        {
            return spark.FindWith(new TFinder());
        }

        public static SparkEngine ComposeWith<TComposer>(this SparkEngine spark)
            where TComposer : ITemplateComposer, new()
        {
            return spark.ComposeWith(new TComposer());
        }

        public static SparkEngine ConfigureComposer<TConvention>(this SparkEngine spark)
            where TConvention : ITemplateComposerConvention, new()
        {
            return spark.ConfigureComposer(new TConvention());
        }

        public static SparkEngine ConfigureComposer(this SparkEngine spark, Action<TemplateComposer> configure)
        {
            return spark.ConfigureComposer(new LambdaTemplateComposerConvention(configure));
        }
    }
}