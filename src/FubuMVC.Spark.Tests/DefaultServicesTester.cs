using System.Linq;
using Bottles;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.View.Model;
using FubuMVC.Core.View.Model.Sharing;
using FubuMVC.Core.View.Rendering;
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
        public void parsing_registration()
        {
            _services.DefaultServiceFor<IParsingRegistrations<ISparkTemplate>>()
                .Value.ShouldBeOfType<Parsings>();
        }

        [Test]
        public void sharing_graph()
        {
            _services.DefaultServiceFor<SharingGraph>().ShouldNotBeNull()
                .Value.ShouldNotBeNull()
                .ShouldEqual(_services.DefaultServiceFor<ISharingGraph>().ShouldNotBeNull().Value);
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
        public void sharing_logs_cache()
        {
            _services.DefaultServiceFor<SharingLogsCache>().ShouldNotBeNull()
                .Value.ShouldNotBeNull().ShouldBeOfType<SharingLogsCache>();
        }

        [Test]
        public void sharing_config_activator()
        {
            defaultServicesCheck<IActivator, SharingConfigActivator>(0);
        }

        [Test]
        public void sharing_policy_activator()
        {
            defaultServicesCheck<IActivator, SharingPolicyActivator>(1);
        }

        [Test]
        public void sharing_attacher_activator()
        {
            defaultServicesCheck<IActivator, SharingAttacherActivator<ISparkTemplate>>(2);
        }

        [Test]
        public void spark_activator()
        {
            defaultServicesCheck<IActivator, SparkActivator>(3);
        }

        [Test]
        public void precompiler_activator()
        {
            defaultServicesCheck<IActivator, SparkPrecompiler>(4);
        }

        [Test]
        public void sharing_attachers()
        {
            countForServiceCheck<ISharingAttacher<ISparkTemplate>>(2);
        }

        [Test]
        public void master_attacher()
        {
            defaultServicesCheck<ISharingAttacher<ISparkTemplate>, MasterAttacher<ISparkTemplate>>(0);
        }

        [Test]
        public void bindings_attacher()
        {
            defaultServicesCheck<ISharingAttacher<ISparkTemplate>, BindingsAttacher>(1);
        }

        [Test]
        public void shared_path_builder()
        {
            _services.DefaultServiceFor<ISharedPathBuilder>()
                .Value.ShouldBeOfType<SharedPathBuilder>();
        }

        [Test]
        public void template_directory_provider()
        {
            defaultServiceCheck<ITemplateDirectoryProvider<ISparkTemplate>, TemplateDirectoryProvider<ISparkTemplate>>();
        }

        [Test]
        public void shared_template_locator()
        {
            defaultServiceCheck<ISharedTemplateLocator, SharedTemplateLocator>();
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