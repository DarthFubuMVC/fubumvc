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
    public class SparkEngineTester : InteractionContext<SparkEngine>
    {
        private IServiceRegistry _services;
        protected override void beforeEach()
        {
            var registry = new FubuRegistry();
            ClassUnderTest.As<IFubuRegistryExtension>().Configure(registry);
            _services = registry.BuildGraph().Services;
        }

        [Test]
        public void template_registry()
        {
            _services.DefaultServiceFor<ITemplateRegistry>().ShouldNotBeNull()
                .Value.ShouldNotBeNull().ShouldBeOfType<TemplateRegistry>();
        }

        [Test]
        public void parsing_registration()
        {
            _services.DefaultServiceFor<IParsingRegistrations>().ShouldNotBeNull()
                .Value.ShouldNotBeNull().ShouldBeOfType<Parsings>();
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
            _services.DefaultServiceFor<ISparkViewEngine>().ShouldNotBeNull()
                .Value.ShouldNotBeNull().ShouldBeOfType<SparkViewEngine>();
        }

        [Test]
        public void cache_service()
        {
            _services.DefaultServiceFor<ICacheService>().ShouldNotBeNull()
                .Value.ShouldNotBeNull().ShouldBeOfType<DefaultCacheService>();
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
            _services.ServicesFor<IActivator>().ElementAtOrDefault(0).ShouldNotBeNull()
                .Type.ShouldEqual(typeof(SharingConfigActivator));
        }

        [Test]
        public void sharing_policy_activator()
        {
            _services.ServicesFor<IActivator>().ElementAtOrDefault(1).ShouldNotBeNull()
                .Type.ShouldEqual(typeof(SharingPolicyActivator));
        }

        [Test]
        public void sharing_attacher_activator()
        {
            _services.ServicesFor<IActivator>().ElementAtOrDefault(2).ShouldNotBeNull()
                .Type.ShouldEqual(typeof(SharingAttacherActivator));
        }

        [Test]
        public void spark_activator()
        {
            _services.ServicesFor<IActivator>().ElementAtOrDefault(3).ShouldNotBeNull()
                .Type.ShouldEqual(typeof(SparkActivator));
        }

        [Test]
        public void sharing_attachers()
        {
            _services.ServicesFor<ISharingAttacher>().ShouldHaveCount(2);
        }

        [Test]
        public void master_attacher()
        {
            _services.ServicesFor<ISharingAttacher>().ElementAtOrDefault(0).ShouldNotBeNull()
                .Type.ShouldEqual(typeof (MasterAttacher));
        }

        [Test]
        public void bindings_attacher()
        {
            _services.ServicesFor<ISharingAttacher>().ElementAtOrDefault(1).ShouldNotBeNull()
                .Type.ShouldEqual(typeof(BindingsAttacher));
        }

        [Test]
        public void shared_path_builder()
        {
            _services.DefaultServiceFor<ISharedPathBuilder>().ShouldNotBeNull()
                .Value.ShouldNotBeNull().ShouldBeOfType<SharedPathBuilder>();
        }

        [Test]
        public void template_directory_provider()
        {
            _services.DefaultServiceFor<ITemplateDirectoryProvider>().ShouldNotBeNull()
                .Type.ShouldEqual(typeof(TemplateDirectoryProvider));
        }

        [Test]
        public void shared_template_locator()
        {
            _services.DefaultServiceFor<ISharedTemplateLocator>().ShouldNotBeNull()
                .Type.ShouldEqual(typeof(SharedTemplateLocator));
        }

        [Test]
        public void render_strategies()
        {
            _services.ServicesFor<IRenderStrategy>().ShouldHaveCount(3);
        }

        [Test]
        public void nested_render_strategy()
        {
            _services.ServicesFor<IRenderStrategy>().ElementAtOrDefault(0)
                .ShouldNotBeNull().Type.ShouldEqual(typeof(NestedRenderStrategy));
        }

        [Test]
        public void ajax_render_strategy()
        {
            _services.ServicesFor<IRenderStrategy>().ElementAtOrDefault(1)
                .ShouldNotBeNull().Type.ShouldEqual(typeof(AjaxRenderStrategy));
        }

        [Test]
        public void default_render_strategy()
        {
            _services.ServicesFor<IRenderStrategy>().ElementAtOrDefault(2)
                .ShouldNotBeNull().Type.ShouldEqual(typeof(DefaultRenderStrategy));
        }

        [Test]
        public void view_entry_provider_cache()
        {
            _services.DefaultServiceFor<IViewEntryProviderCache>().ShouldNotBeNull()
                .Type.ShouldEqual(typeof (ViewEntryProviderCache));
        }

        [Test]
        public void view_modifier_service()
        {
            _services.DefaultServiceFor<IViewModifierService>().ShouldNotBeNull()
                .Type.ShouldEqual(typeof(ViewModifierService));
        }

        [Test]
        public void view_modifiers()
        {
            _services.ServicesFor<IViewModifier>().ShouldHaveCount(8);
        }

        [Test]
        public void page_activation_view_modifier()
        {
            _services.ServicesFor<IViewModifier>().ElementAtOrDefault(0).ShouldNotBeNull()
                .Type.ShouldEqual(typeof(PageActivation));
        }

        [Test]
        public void site_resource_attacher_view_modifier()
        {
            _services.ServicesFor<IViewModifier>().ElementAtOrDefault(1).ShouldNotBeNull()
                .Type.ShouldEqual(typeof(SiteResourceAttacher));
        }

        [Test]
        public void content_activation_view_modifier()
        {
            _services.ServicesFor<IViewModifier>().ElementAtOrDefault(2).ShouldNotBeNull()
                .Type.ShouldEqual(typeof(ContentActivation));
        }

        [Test]
        public void once_table_activation_view_modifier()
        {
            _services.ServicesFor<IViewModifier>().ElementAtOrDefault(3).ShouldNotBeNull()
                .Type.ShouldEqual(typeof(OnceTableActivation));
        }

        [Test]
        public void outer_view_output_activator_view_modifier()
        {
            _services.ServicesFor<IViewModifier>().ElementAtOrDefault(4).ShouldNotBeNull()
                .Type.ShouldEqual(typeof(OuterViewOutputActivator));
        }

        [Test]
        public void nested_view_output_activator_view_modifier()
        {
            _services.ServicesFor<IViewModifier>().ElementAtOrDefault(5).ShouldNotBeNull()
                .Type.ShouldEqual(typeof(NestedViewOutputActivator));
        }

        [Test]
        public void view_content_disposer_view_modifier()
        {
            _services.ServicesFor<IViewModifier>().ElementAtOrDefault(6).ShouldNotBeNull()
                .Type.ShouldEqual(typeof(ViewContentDisposer));
        }

        [Test]
        public void nested_output_activation_view_modifier()
        {
            _services.ServicesFor<IViewModifier>().ElementAtOrDefault(7).ShouldNotBeNull()
                .Type.ShouldEqual(typeof(NestedOutputActivation));
        }

        [Test]
        public void html_encoder()
        {
            _services.DefaultServiceFor<IHtmlEncoder>().ShouldNotBeNull()
                .Type.ShouldEqual(typeof (DefaultHtmlEncoder));
        }

        [Test]
        public void default_view_definition_policy()
        {
            _services.DefaultServiceFor<DefaultViewDefinitionPolicy>().ShouldNotBeNull()
                .Value.ShouldNotBeNull();
        }

        [Test]
        public void view_definition_resolver()
        {
            _services.DefaultServiceFor<IViewDefinitionResolver>().ShouldNotBeNull()
                .Type.ShouldEqual(typeof (ViewDefinitionResolver));
        }
    }
}