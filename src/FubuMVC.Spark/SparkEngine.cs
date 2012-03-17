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
using FubuMVC.Core.View.Conventions;
using FubuMVC.Core.View.Model;
using FubuMVC.Core.View.Model.Sharing;
using FubuMVC.Core.View.Rendering;
using FubuMVC.Spark.Rendering;
using FubuMVC.Spark.SparkModel;
using Spark;
using Spark.Caching;

namespace FubuMVC.Spark
{
    // Subject for major rip apart: Separate bare template scanning and view attachment from master/bindings concerns.
    // TODO: Move parts that are possible to IActivator such that we can build up a model per package.

    public class SparkEngine : IFubuRegistryExtension
    {
        private static bool _hasScanned;
        private static readonly Parsings _parsings = new Parsings();
        private static readonly SparkTemplateRegistry _templateRegistry = new SparkTemplateRegistry();
        private static readonly Lazy<TypePool> _types = new Lazy<TypePool>(getTypes);

        private ITemplateFinder<Template> _finder;
        private ITemplateComposer<ITemplate> _composer;
        private readonly IPackageLog _logger;
        private readonly IList<ITemplateFinderConvention<Template>> _finderConventions = new List<ITemplateFinderConvention<Template>>();
        private readonly IList<ITemplateComposerConvention<ITemplate>> _composerConventions = new List<ITemplateComposerConvention<ITemplate>>();

        public SparkEngine()
        {
            _logger = getLogger();
            _finder = new TemplateFinder<Template>();
            _composer = new TemplateComposer<ITemplate>(_types.Value, _parsings);

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
                    .AddBinder<GenericViewModelBinder<ITemplate>>()
                    .AddBinder<ViewModelBinder<ITemplate>>()
                    .Apply<NamespacePolicy>()
                    .Apply<ViewPathPolicy<ITemplate>>());
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
            parseTemplates();
            composeTemplates();

            _hasScanned = true;
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

        private void parseTemplates()
        {
            _templateRegistry.Each(t => _parsings.Process(t));
        }

        private void composeTemplates()
        {
            var composer = _composer as TemplateComposer<ITemplate>;
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

        private static void configureServices(IServiceRegistry services)
        {
            services.SetServiceIfNone<ISparkTemplateRegistry>(_templateRegistry);
            services.SetServiceIfNone<ITemplateRegistry<ITemplate>>(_templateRegistry);
            services.SetServiceIfNone<IParsingRegistrations<ITemplate>>(_parsings);
            
            var graph = new SharingGraph();
            services.SetServiceIfNone(graph);
            services.SetServiceIfNone<ISharingGraph>(graph);

            services.SetServiceIfNone<ISparkViewEngine>(new SparkViewEngine());
            services.SetServiceIfNone<ICacheService>(new DefaultCacheService(HttpRuntime.Cache));

            services.SetServiceIfNone(new SharingLogsCache());

            services.FillType<IActivator, SharingConfigActivator>();
            services.FillType<IActivator, SharingPolicyActivator>();
            services.FillType<IActivator, SharingAttacherActivator<ITemplate>>();
            services.FillType<IActivator, SparkActivator>();

            services.FillType<ISharingAttacher<ITemplate>, MasterAttacher<ITemplate>>();
            services.FillType<ISharingAttacher<ITemplate>, BindingsAttacher>();

            services.SetServiceIfNone<ISharedPathBuilder>(new SharedPathBuilder());
            services.SetServiceIfNone<ITemplateDirectoryProvider<ITemplate>, TemplateDirectoryProvider<ITemplate>>();
            services.SetServiceIfNone<ISharedTemplateLocator, SharedTemplateLocator>();
            services.FillType<ISharedTemplateLocator<ITemplate>, SharedTemplateLocator>();

            services.FillType<IRenderStrategy, NestedRenderStrategy>();
            services.FillType<IRenderStrategy, AjaxRenderStrategy>();
            services.FillType<IRenderStrategy, DefaultRenderStrategy>();

            services.FillType<ITemplateSelector<ITemplate>, SparkTemplateSelector>();

            services.SetServiceIfNone<IViewEntryProviderCache, ViewEntryProviderCache>();
            services.SetServiceIfNone<IViewModifierService<IFubuSparkView>, ViewModifierService<IFubuSparkView>>();

            services.FillType<IViewModifier<IFubuSparkView>, PageActivation<IFubuSparkView>>();
            services.FillType<IViewModifier<IFubuSparkView>, SiteResourceAttacher>();
            services.FillType<IViewModifier<IFubuSparkView>, ContentActivation>();
            services.FillType<IViewModifier<IFubuSparkView>, OnceTableActivation>();
            services.FillType<IViewModifier<IFubuSparkView>, OuterViewOutputActivator>();
            services.FillType<IViewModifier<IFubuSparkView>, NestedViewOutputActivator>();
            services.FillType<IViewModifier<IFubuSparkView>, ViewContentDisposer>();
            services.FillType<IViewModifier<IFubuSparkView>, NestedOutputActivation>();

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

        public SparkEngine FindWith(ITemplateFinder<Template> finder)
        {
            _finder = finder;
            return this;
        }

        public SparkEngine ConfigureFinder(ITemplateFinderConvention<Template> convention)
        {
            _finderConventions.Fill(convention);
            return this;
        }

        public SparkEngine ComposeWith(ITemplateComposer<ITemplate> composer)
        {
            _composer = composer;
            return this;
        }

        public SparkEngine ConfigureComposer(ITemplateComposerConvention<ITemplate> convention)
        {
            _composerConventions.Fill(convention);
            return this;
        }
    }

    public static class SparkExtensions
    {
        public static IList<ITemplateFinderConvention<Template>> Apply(this IList<ITemplateFinderConvention<Template>> source, Action<TemplateFinder<Template>> configure)
        {
            source.Fill(new LambdaTemplateFinderConvention<Template>(configure));
            return source;
        }

        public static IList<ITemplateComposerConvention<ITemplate>> Apply(this IList<ITemplateComposerConvention<ITemplate>> source, Action<TemplateComposer<ITemplate>> configure)
        {
            source.Fill(new LambdaTemplateComposerConvention<ITemplate>(configure));
            return source;
        }

        public static SparkEngine ConfigureFinder<TConvention>(this SparkEngine spark)
            where TConvention : ITemplateFinderConvention<Template>, new()
        {
            return spark.ConfigureFinder(new TConvention());
        }

        public static SparkEngine ConfigureFinder(this SparkEngine spark, Action<TemplateFinder<Template>> configure)
        {
            return spark.ConfigureFinder(new LambdaTemplateFinderConvention<Template>(configure));
        }

        public static SparkEngine FindWith<TFinder>(this SparkEngine spark)
            where TFinder : ITemplateFinder<Template>, new()
        {
            return spark.FindWith(new TFinder());
        }

        public static SparkEngine ComposeWith<TComposer>(this SparkEngine spark)
            where TComposer : ITemplateComposer<ITemplate>, new()
        {
            return spark.ComposeWith(new TComposer());
        }

        public static SparkEngine ConfigureComposer<TConvention>(this SparkEngine spark)
            where TConvention : ITemplateComposerConvention<ITemplate>, new()
        {
            return spark.ConfigureComposer(new TConvention());
        }

        public static SparkEngine ConfigureComposer(this SparkEngine spark, Action<TemplateComposer<ITemplate>> configure)
        {
            return spark.ConfigureComposer(new LambdaTemplateComposerConvention<ITemplate>(configure));
        }
    }
}