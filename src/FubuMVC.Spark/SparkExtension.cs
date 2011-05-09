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
        private readonly SparkTemplates _templates;
        private SparkItemComposer _itemComposer;
        private TemplateFinder _templateFinder;
        
        public SparkExtension()
        {
			_templates = new SparkTemplates();
			_templateFinder = new TemplateFinder();
            _itemComposer = new SparkItemComposer(_templates);
			
			defaults();
        }

        public void Configure(FubuRegistry registry)
        {
            locateTemplates();
			
            registry.Views.Facility(new SparkViewFacility(_itemComposer));
            registry.Services(configureServices);
        }

        private void configureServices(IServiceRegistry services)
        {
            // TODO : Reconsider this
            services.SetServiceIfNone<ISparkTemplates>(_templates);
            services.SetServiceIfNone<ISparkViewEngine>(new SparkViewEngine());            
            services.AddService<IActivator, SparkActivator>();
            services.AddService<ISparkViewModification, PageActivation>();
            services.AddService<ISparkViewModification, SiteResourceAttacher>();
        }

        private void locateTemplates()
        {
            _templates.Clear();
            _templates.AddRange(_templateFinder.FindInHost());
            _templates.AddRange(_templateFinder.FindInPackages());
        }
		
		private void defaults()
		{			
			_itemComposer
                .AddBinder<ViewDescriptorBinder>()
                .AddBinder<MasterPageBinder>()
                .AddBinder<ViewModelBinder>()
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