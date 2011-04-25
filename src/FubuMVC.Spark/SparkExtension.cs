using System;
using Bottles;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Spark.SparkModel;
using Spark;

namespace FubuMVC.Spark
{
    // This approach uses default conventions.
    public class SparkExtension : IFubuRegistryExtension, ISparkExtension
    {
        private readonly SparkItems _sparkItems;
        private readonly ISparkViewEngine _sparkViewEngine;
        private readonly SparkItemBuilder _tokenizer;
        private readonly SparkItemFinder _sparkItemFinder;
        public SparkExtension()
        {
            _sparkItemFinder = new SparkItemFinder();
            // TODO: move onto conventions
            _tokenizer = new SparkItemBuilder(_sparkItemFinder, new ChunkLoader())
                .Apply<MasterPageBinder>()
                .Apply<ViewModelBinder>()
                .Apply<NamespaceBinder>()
                .Apply<ViewPathBinder>();

            _sparkItems = new SparkItems();
            _sparkViewEngine = new SparkViewEngine();
        }

        public void Include(string filter)
        {
            _sparkItemFinder.Include(filter);
        }

        public void Configure(FubuRegistry registry)
        {
            var facility = new SparkViewFacility(_tokenizer, _sparkItems);
            registry.Services(x => x.SetServiceIfNone(_sparkItems));
            registry.Services(x => x.SetServiceIfNone(_sparkViewEngine));
            registry.Services(x => x.AddService<IActivator, SparkActivator>());
            registry.Views.Facility(facility);            
        }

        // DSL 
        // ConfigureSparkExpression...
    }

    public interface ISparkExtension
    {
        // NOTE:TEMP
        void Include(string filter);
    }

    // TODO: Ask JDM & JA about this.
    // This approach allows for configuration.
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