using System;
using Bottles;
using FubuMVC.Core;
using FubuMVC.Spark.SparkModel;
using Spark;

namespace FubuMVC.Spark
{
    // This approach uses default conventions.
    public class SparkExtension : IFubuRegistryExtension, ISparkExtension
    {
        private readonly SparkItems _sparkItems;
        private readonly SparkItemBuilder _itemBuilder;
        private readonly SparkItemFinder _sparkItemFinder;
        
        public SparkExtension()
        {
            _sparkItems = new SparkItems();
            _sparkItemFinder = new SparkItemFinder();
            
            _itemBuilder = new SparkItemBuilder(_sparkItems)
                .AddBinder<MasterPageBinder>()
                .AddBinder<ViewModelBinder>()
                .Apply<NamespacePolicy>()
                .Apply<ViewPathPolicy>();
        }

        public void Include(string filter)
        {
            _sparkItemFinder.Include(filter);
        }

        public void Configure(FubuRegistry registry)
        {
            populateAndBuildItems();
            registry.Views.Facility(new SparkViewFacility(_sparkItems));
            
            services(registry);
        }
        
        private void populateAndBuildItems()
        {
            _sparkItems.Clear();
            _sparkItems.AddRange(_sparkItemFinder.FindItems());
            _itemBuilder.BuildItems();
        }

        private void services(IFubuRegistry registry)
        {
            registry.Services(x => x.SetServiceIfNone(_sparkItems));
            registry.Services(x => x.SetServiceIfNone<ISparkViewEngine>(new SparkViewEngine()));
            registry.Services(x => x.AddService<IActivator, SparkActivator>());            
        }

        // DSL 
        // ConfigureSparkExpression...
    }

    public interface ISparkExtension
    {
        // NOTE:TEMP
        void Include(string filter);
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