using System.Linq;
using Bottles;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Spark.Rendering;
using FubuMVC.Spark.SparkModel;
using FubuMVC.Spark.SparkModel.Sharing;
using FubuTestingSupport;
using NUnit.Framework;
using Spark;
using Spark.Caching;

namespace FubuMVC.Spark.Tests
{
    [TestFixture]
    public class DefaultServicesTester : InteractionContext<SparkEngine>
    {
        private IServiceRegistry _services;

        protected override void beforeEach()
        {
            var registry = new FubuRegistry();
            ClassUnderTest.As<IFubuRegistryExtension>().Configure(registry);
            _services = registry.BuildLightGraph().Services;
        }

        [Test]
        public void template_registry()
        {
            _services.DefaultServiceFor<ITemplateRegistry>()
                .Value.ShouldBeOfType<TemplateRegistry>();
        }

        [Test]
        public void parsing_registration()
        {
            _services.DefaultServiceFor<IParsingRegistrations>()
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
            defaultServicesCheck<IActivator, SharingAttacherActivator>(2);
        }

        [Test]
        public void spark_activator()
        {
            defaultServicesCheck<IActivator, SparkActivator>(3);
        }

        [Test]
        public void sharing_attachers()
        {
            countForServiceCheck<ISharingAttacher>(2);
        }

        [Test]
        public void master_attacher()
        {
            defaultServicesCheck<ISharingAttacher, MasterAttacher>(0);
        }

        [Test]
        public void bindings_attacher()
        {
            defaultServicesCheck<ISharingAttacher, BindingsAttacher>(1);
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
            defaultServiceCheck<ITemplateDirectoryProvider, TemplateDirectoryProvider>();
        }

        [Test]
        public void shared_template_locator()
        {
            defaultServiceCheck<ISharedTemplateLocator, SharedTemplateLocator>();
        }

        [Test]
        public void render_strategies()
        {
            countForServiceCheck<IRenderStrategy>(3);
        }

        [Test]
        public void nested_render_strategy()
        {
            defaultServicesCheck<IRenderStrategy, NestedRenderStrategy>(0);
        }

        [Test]
        public void ajax_render_strategy()
        {
            defaultServicesCheck<IRenderStrategy, AjaxRenderStrategy>(1);
        }

        [Test]
        public void default_render_strategy()
        {
            defaultServicesCheck<IRenderStrategy, DefaultRenderStrategy>(2);
        }

        [Test]
        public void view_entry_provider_cache()
        {
            defaultServiceCheck<IViewEntryProviderCache, ViewEntryProviderCache>();
        }

        [Test]
        public void view_modifier_service()
        {
            defaultServiceCheck<IViewModifierService, ViewModifierService>();
        }

        [Test]
        public void view_modifiers()
        {
            countForServiceCheck<IViewModifier>(8);
        }

        [Test]
        public void page_activation_view_modifier()
        {
            defaultServicesCheck<IViewModifier, PageActivation>(0);
        }

        [Test]
        public void site_resource_attacher_view_modifier()
        {
            defaultServicesCheck<IViewModifier, SiteResourceAttacher>(1);
        }

        [Test]
        public void content_activation_view_modifier()
        {
            defaultServicesCheck<IViewModifier, ContentActivation>(2);
        }

        [Test]
        public void once_table_activation_view_modifier()
        {
            defaultServicesCheck<IViewModifier, OnceTableActivation>(3);
        }

        [Test]
        public void outer_view_output_activator_view_modifier()
        {
            defaultServicesCheck<IViewModifier, OuterViewOutputActivator>(4);
        }

        [Test]
        public void nested_view_output_activator_view_modifier()
        {
            defaultServicesCheck<IViewModifier, NestedViewOutputActivator>(5);
        }

        [Test]
        public void view_content_disposer_view_modifier()
        {
            defaultServicesCheck<IViewModifier, ViewContentDisposer>(6);
        }

        [Test]
        public void nested_output_activation_view_modifier()
        {
            defaultServicesCheck<IViewModifier, NestedOutputActivation>(7);
        }

        [Test]
        public void html_encoder()
        {
            defaultServiceCheck<IHtmlEncoder, DefaultHtmlEncoder>();
        }

        [Test]
        public void default_view_definition_policy()
        {
            defaultServiceCheck<DefaultViewDefinitionPolicy>();
        }

        [Test]
        public void view_definition_resolver()
        {
            defaultServiceCheck<IViewDefinitionResolver, ViewDefinitionResolver>();
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

        private void defaultServiceCheck<TImplementation>()
        {
            _services.DefaultServiceFor<TImplementation>()
                .ShouldNotBeNull().Value.ShouldNotBeNull();
        }

        private void defaultServiceCheck<TService, TImplementation>()
        {
            _services.DefaultServiceFor<TService>().ShouldNotBeNull()
                .Type.ShouldEqual(typeof(TImplementation));
        }
    }
}