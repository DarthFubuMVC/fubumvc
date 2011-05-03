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
        private SparkItemBuilder _itemBuilder;
        private SparkItemFinder _itemFinder;
        
        public SparkExtension()
        {
			_items = new SparkItems();
			_itemFinder = new SparkItemFinder();
            _itemBuilder = new SparkItemBuilder(_items);
			
			defaults();
        }
        public void Configure(FubuRegistry registry)
        {
            populateAndBuildItems();
			
            registry.Views.Facility(new SparkViewFacility(_items));
            registry.Services(configureServices);
        }

        private void configureServices(IServiceRegistry services)
        {
            services.SetServiceIfNone<ISparkItems>(_items);
            services.SetServiceIfNone<ISparkViewEngine>(new SparkViewEngine());            
            services.AddService<IActivator, SparkActivator>();
            services.AddService<ISparkViewModification, ServiceLocatorAttacher>();
            services.AddService<ISparkViewModification, ModelAttacher>();
            services.AddService<ISparkViewModification, SiteResourceAttacher>();
        }

        private void populateAndBuildItems()
        {
            _items.Clear();
            _items.AddRange(_itemFinder.FindInHost());
            _items.AddRange(_itemFinder.FindInPackages());
            _itemBuilder.BuildItems();
        }
		
		private void defaults()
		{			
			_itemBuilder
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