using System.Linq;
using Bottles;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.View.Model;
using FubuMVC.Spark.Rendering;
using FubuMVC.Spark.SparkModel;
using FubuTestingSupport;
using NUnit.Framework;
using Spark;
using Spark.Caching;

namespace FubuMVC.Spark.Tests
{
    [TestFixture]
    public class DefaultServicesTester : InteractionContext<SparkEngine>
    {
        private ServiceGraph _services;

        protected override void beforeEach()
        {
            var registry = new FubuRegistry();
            ClassUnderTest.As<IFubuRegistryExtension>().Configure(registry);
            _services = BehaviorGraph.BuildFrom(registry).Services;

            
        }

        [Test]
        public void template_registry()
        {
            _services.DefaultServiceFor<ITemplateRegistry<ISparkTemplate>>()
                .Value.ShouldBeOfType<TemplateRegistry<ISparkTemplate>>();
        }

        [Test]
        public void spark_view_engine_service()
        {
            _services.DefaultServiceFor<ISparkViewEngine>()
                .Value.ShouldBeOfType<SparkViewEngine>();
        }

        [Test]
        public void cache_service()
        {
            _services.DefaultServiceFor<ICacheService>()
                .Value.ShouldBeOfType<DefaultCacheService>();
        }

        [Test]
        public void spark_activator()
        {
            defaultServicesCheck<IActivator, SparkActivator>(3);
        }


        [Test]
        public void html_encoder()
        {
            defaultServiceCheck<IHtmlEncoder, DefaultHtmlEncoder>();
        }

        private void countForServiceCheck<TService>(int count)
        {
            _services.ServicesFor<TService>().ShouldHaveCount(count);
        }

        private void defaultServicesCheck<TService, TImplementation>(int position)
        {
            _services.ServicesFor<TService>().ElementAtOrDefault(position)
                .ShouldNotBeNull().Type.ShouldEqual(typeof (TImplementation));
        }


        private void defaultServiceCheck<TService, TImplementation>()
        {
            _services.DefaultServiceFor<TService>().ShouldNotBeNull()
                .Type.ShouldEqual(typeof(TImplementation));
        }
    }
}