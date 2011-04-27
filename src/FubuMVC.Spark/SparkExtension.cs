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

            registry.Services(configureServices);
        }

        private void configureServices(IServiceRegistry services)
        {
            services.SetServiceIfNone<ISparkItems>(_sparkItems);
            services.SetServiceIfNone<ISparkViewEngine>(new SparkViewEngine());
            
            services.AddService<IActivator, SparkActivator>();
            services.AddService<ISparkViewModification, ServiceLocatorAttacher>();
            services.AddService<ISparkViewModification, ModelAttacher>();
            services.SetServiceIfNone<IRenderInfo, RenderInfo>();
        }

        private void populateAndBuildItems()
        {
            _sparkItems.Clear();
            _sparkItems.AddRange(_sparkItemFinder.FindInHost());
            _sparkItems.AddRange(_sparkItemFinder.FindInPackages());
            _itemBuilder.BuildItems();
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