using System;
using System.Collections.Generic;
using Bottles;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Spark.Rendering;
using FubuMVC.Spark.SparkModel;
using Spark;

namespace FubuMVC.Spark
{
    public class SparkExtension : IFubuRegistryExtension, ISparkExtension
    {
        private readonly TemplateRegistry _templateRegistry;
        private readonly TemplateComposer _composer;
        private readonly TemplateFinder _finder;
        private readonly TypePool _types = new TypePool(FubuRegistry.FindTheCallingAssembly()) { ShouldScanAssemblies = true };
        
        public SparkExtension()
        {
			_templateRegistry = new TemplateRegistry();
			_finder = new TemplateFinder();
            _composer = new TemplateComposer(_types);
			
			defaults();
        }

        public void Configure(FubuRegistry registry)
        {
            // Consider throwing custom exception if registry is IFubuRegistryExtension

            populateTypes(registry.Types);
            composeTemplates();			
            registry.Views.Facility(new SparkViewFacility(_templateRegistry));
            
            registry.Services(configureServices);
        }

        private void composeTemplates()
        {
            _templateRegistry.Clear();
            _templateRegistry.AddRange(_finder.FindInHost());
            _templateRegistry.AddRange(_finder.FindInPackages());

            _composer.Compose(_templateRegistry);
        }

        private void populateTypes(TypePool importerTypes)
        {
            importerTypes.TypesMatching(t => true).Each(_types.AddType);
            _types.AddSource(() => importerTypes.Assemblies);
            _types.AddSource(() => PackageRegistry.PackageAssemblies);
            _types.AddAssembly(typeof(FubuApplication).Assembly); // In case of output types are needed from FubuMVC.Core
        }

        private void defaults()
        {
            _composer
                .AddBinder<ViewDescriptorBinder>()
                .AddBinder<MasterPageBinder>()
                .AddBinder<ViewModelBinder>()
                .AddBinder<ReachableBindingsBinder>()
                .Apply<NamespacePolicy>()
                .Apply<ViewPathPolicy>();
        }

        private void configureServices(IServiceRegistry services)
        {
            services.SetServiceIfNone<ITemplateRegistry>(_templateRegistry);
            
            services.SetServiceIfNone<ISparkViewEngine>(new SparkViewEngine());            
            services.AddService<IActivator, SparkActivator>();

            services.AddService<IRenderStrategy, NestedRenderStrategy>();
            services.AddService<IRenderStrategy, AjaxRenderStrategy>();
            services.AddService<IRenderStrategy, DefaultRenderStrategy>();

            services.SetServiceIfNone<IViewEntryProvider, ViewEntryProviderCache>();
            services.SetServiceIfNone<IViewModifierService, ViewModifierService>();

            services.AddService<IViewModifier, PageActivation>();
            services.AddService<IViewModifier, SiteResourceAttacher>();
            services.AddService<IViewModifier, ContentActivation>();
            services.AddService<IViewModifier, OnceTableActivation>();
            services.AddService<IViewModifier, OuterViewOutputActivator>();
            services.AddService<IViewModifier, NestedViewOutputActivator>();
            services.AddService<IViewModifier, ViewContentDisposer>();
            services.AddService<IViewModifier, NestedOutputActivation>();
        }
		
        // DSL 
        // ConfigureSparkExpression...
    }

    public interface ISparkExtension
    {
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
}