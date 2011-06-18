using System;
using System.Linq;
using Bottles;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Spark.Rendering;
using FubuMVC.Spark.SparkModel;
using Spark;
using FubuCore.Util;
using FubuCore;
using System.Reflection;
using System.Threading;
using Bottles.Diagnostics;

namespace FubuMVC.Spark
{
    public class SparkExtension : IFubuRegistryExtension, ISparkExtension
    {
        private static bool _hasScanned;
        private static readonly object _lock = new object();
        private static readonly TemplateRegistry _templateRegistry = new TemplateRegistry();
        private static readonly Lazy<TypePool> _types = new Lazy<TypePool>(getTypes);

        private TemplateFinder _finder;
        private TemplateComposer _composer;
        private IPackageLog _logger;
        
        public SparkExtension()
        {
            _logger = PackageRegistry.Diagnostics.LogFor(this);
            _finder = new TemplateFinder();            
            _composer = new TemplateComposer(_types.Value);
        }

        public void Configure(FubuRegistry registry)
        {
            lock (_lock)
            {
                if (shouldScan()) { scan(registry); }
            }

            registry.Views.Facility(new SparkViewFacility(_templateRegistry));            
            registry.Services(configureServices);
        }

        private bool shouldScan()
        {
            return !_hasScanned;
        }

        private void scan(FubuRegistry parentRegistry)
        {
            var msg = "Initializing Spark in: [{0}]".ToFormat(parentRegistry.Name);
            _logger.Trace(ConsoleColor.Green, msg);

            findTemplates();
            composeTemplates();

            _hasScanned = true;
        }

        private void findTemplates()
        {
            _templateRegistry.Clear();
            _templateRegistry.AddRange(_finder.FindInHost());
            _templateRegistry.AddRange(_finder.FindInPackages());   
        }

        private void composeTemplates()
        {
            _composer
                .AddBinder<ViewDescriptorBinder>()
                .AddBinder<MasterPageBinder>()
                .AddBinder<ViewModelBinder>()
                .AddBinder<ReachableBindingsBinder>()
                .Apply<NamespacePolicy>()
                .Apply<ViewPathPolicy>();

            _composer.Compose(_templateRegistry);
        }

        private static TypePool getTypes()
        {
            var types = new TypePool(FubuRegistry.FindTheCallingAssembly())
            {
                ShouldScanAssemblies = true
            };

            var filter = new CompositeFilter<Assembly>();

            filter.Excludes.Add(a => a.IsDynamic);
            filter.Excludes.Add(a => types.HasAssembly(a));

            types.AddSource(() => AppDomain.CurrentDomain.GetAssemblies().Where(filter.MatchesAll));

            return types;
        }

        private void configureServices(IServiceRegistry services)
        {
            services.SetServiceIfNone<ITemplateRegistry>(_templateRegistry);            
            services.SetServiceIfNone<ISparkViewEngine>(new SparkViewEngine());            

            services.FillType<IActivator, SparkActivator>();

            services.FillType<IRenderStrategy, NestedRenderStrategy>();
            services.FillType<IRenderStrategy, AjaxRenderStrategy>();
            services.FillType<IRenderStrategy, DefaultRenderStrategy>();

            services.SetServiceIfNone<IViewEntryProvider, ViewEntryProviderCache>();
            services.SetServiceIfNone<IViewModifierService, ViewModifierService>();

            services.FillType<IViewModifier, PageActivation>();
            services.FillType<IViewModifier, SiteResourceAttacher>();
            services.FillType<IViewModifier, ContentActivation>();
            services.FillType<IViewModifier, OnceTableActivation>();
            services.FillType<IViewModifier, OuterViewOutputActivator>();
            services.FillType<IViewModifier, NestedViewOutputActivator>();
            services.FillType<IViewModifier, ViewContentDisposer>();
            services.FillType<IViewModifier, NestedOutputActivation>();
        }		
    }

    public interface ISparkExtension
    {
        // We need expressions here such that template finder + composer can be configured/replaced
    }

    public static class FubuRegistryExtensions
    {
        public static void UseSpark(this FubuRegistry fubuRegistry)
        {
            fubuRegistry.UseSpark(s => {});    
        }

        public static void UseSpark(this FubuRegistry fubuRegistry, Action<ISparkExtension> configure)
        {
            var spark = new SparkExtension();
            configure(spark);
            spark.Configure(fubuRegistry);
        }
    }

    public static class ServiceRegistryExtensions
    {
        public static void FillType<TInterface, TConcrete>(this IServiceRegistry registry)
        {
            registry.FillType(typeof(TInterface), typeof(TConcrete));
        }
    }
}