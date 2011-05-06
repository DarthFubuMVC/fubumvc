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
        private readonly SparkItems _items;
        private SparkItemComposer _itemComposer;
        private TemplateFinder _templateFinder;
        
        public SparkExtension()
        {
			_items = new SparkItems();
			_templateFinder = new TemplateFinder();
            _itemComposer = new SparkItemComposer(_items);
			
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
            services.SetServiceIfNone<ISparkItems>(_items);
            services.SetServiceIfNone<ISparkViewEngine>(new SparkViewEngine());            
            services.AddService<IActivator, SparkActivator>();
            services.AddService<ISparkViewModification, PageActivation>();
            services.AddService<ISparkViewModification, SiteResourceAttacher>();
        }

        private void locateTemplates()
        {
            _items.Clear();
            _items.AddRange(_templateFinder.FindInHost());
            _items.AddRange(_templateFinder.FindInPackages());
        }
		
		private void defaults()
		{			
			_itemComposer
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