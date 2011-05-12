using System;
using Bottles;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Spark.Rendering;
using FubuMVC.Spark.SparkModel;
using Spark;

namespace FubuMVC.Spark
{
    // This approach uses default conventions.
    public class SparkExtension : IFubuRegistryExtension, ISparkExtension
    {
        private readonly TemplateRegistry _templateRegistry;
        private TemplateComposer _composer;
        private TemplateFinder _finder;
        
        public SparkExtension()
        {
			_templateRegistry = new TemplateRegistry();
			_finder = new TemplateFinder();
            _composer = new TemplateComposer(_templateRegistry);
			
			defaults();
        }

        public void Configure(FubuRegistry registry)
        {
            locateTemplates();
			
            registry.Views.Facility(new SparkViewFacility(_composer));
            registry.Services(configureServices);
        }

        private void configureServices(IServiceRegistry services)
        {
            // TODO : Reconsider this
            services.SetServiceIfNone<ITemplateRegistry>(_templateRegistry);
            
            services.SetServiceIfNone<ISparkViewEngine>(new SparkViewEngine());            
            services.AddService<IActivator, SparkActivator>();
            
            services.AddService<ISparkViewModification, PageActivation>();
            services.AddService<ISparkViewModification, SiteResourceAttacher>();
        }

        private void locateTemplates()
        {
            _templateRegistry.Clear();
            _templateRegistry.AddRange(_finder.FindInHost());
            _templateRegistry.AddRange(_finder.FindInPackages());
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