using System.Linq;
using Bottles;
using FubuCore;
using FubuCore.Binding;
using FubuCore.Configuration;
using FubuMVC.Core;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Combination;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Assets.Tags;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Content;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Resources.Media;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security;
using FubuMVC.Core.SessionState;
using FubuMVC.Core.UI;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Activation;
using FubuMVC.Core.Web.Security;
using FubuMVC.WebForms;
using FubuTestingSupport;
using NUnit.Framework;
using IPackageFiles = FubuMVC.Core.Packaging.IPackageFiles;

namespace FubuMVC.Tests.Registration
{
    [TestFixture]
    public class default_fubu_registry_service_registrations
    {
        private void registeredTypeIs<TService, TImplementation>()
        {
            new FubuRegistry().BuildGraph().Services.DefaultServiceFor<TService>().Type.ShouldEqual(
                typeof (TImplementation));
        }

        [Test]
        public void default_binding_logger_is_nullo()
        {
            registeredTypeIs<IBindingLogger, NulloBindingLogger>();
        }

        [Test]
        public void http_output_writer_is_registered()
        {
            registeredTypeIs<IHttpOutputWriter, AspNetHttpOutputWriter>();
        }

        [Test]
        public void url_registry_is_registered()
        {
            registeredTypeIs<IUrlRegistry, UrlRegistry>();
        }

        [Test]
        public void IAuthenticationContext_is_registered()
        {
            registeredTypeIs<IAuthenticationContext, WebAuthenticationContext>();
        }

        [Test]
        public void IFlash_is_registered()
        {
            registeredTypeIs<IFlash, FlashProvider>();
        }

        [Test]
        public void IObjectResolver_is_registered()
        {
            registeredTypeIs<IObjectResolver, ObjectResolver>();
        }

        [Test]
        public void IOutputWriter_is_registered()
        {
            registeredTypeIs<IOutputWriter, OutputWriter>();
        }

        [Test]
        public void IPartialFactory_is_registered()
        {
            registeredTypeIs<IPartialFactory, PartialFactory>();
        }

        [Test]
        public void IRequestData_is_registered()
        {
            registeredTypeIs<IRequestData, RequestData>();
        }

        [Test]
        public void IRequestDataProvider_is_registered()
        {
            registeredTypeIs<IRequestDataProvider, RequestDataProvider>();
        }

        [Test]
        public void ISecurityContext_is_registered()
        {
            registeredTypeIs<ISecurityContext, WebSecurityContext>();
        }


        [Test]
        public void ValueConverterRegistry_is_registered()
        {
            registeredTypeIs<IValueConverterRegistry, ValueConverterRegistry>();
        }

        [Test]
        public void BindingContext_is_registered()
        {
            registeredTypeIs<IBindingContext, BindingContext>();
        }


        [Test]
        public void property_cache_is_registered()
        {
            registeredTypeIs<IPropertyBinderCache, PropertyBinderCache>();
        }

        [Test]
        public void model_binder_cache_is_registered()
        {
            registeredTypeIs<IModelBinderCache, ModelBinderCache>();
        }

        [Test]
        public void streaming_data_is_registered()
        {
            registeredTypeIs<IStreamingData, StreamingData>();
        }

        [Test]
        public void default_json_reader_is_JavascriptDeserializer_flavor()
        {
            registeredTypeIs<IJsonReader, JavaScriptJsonReader>();
        }

        [Test]
        public void display_formatter_is_registered()
        {
            registeredTypeIs<IDisplayFormatter, DisplayFormatter>();
        }

        [Test]
        public void default_authorization_failure_handler_is_registered()
        {
            registeredTypeIs<IAuthorizationFailureHandler, DefaultAuthorizationFailureHandler>();
        }

        [Test]
        public void default_chain_resolver_is_registered()
        {
            registeredTypeIs<IChainResolver, ChainResolver>();
        }

        [Test]
        public void default_endpoint_factory_is_registered()
        {
            registeredTypeIs<IEndPointAuthorizorFactory, EndPointAuthorizorFactory>();
        }

        [Test]
        public void authorization_preview_service_is_registered()
        {
            registeredTypeIs<IAuthorizationPreviewService, AuthorizationPreviewService>();
        }

        [Test]
        public void endpoint_service_is_registered()
        {
            registeredTypeIs<IEndpointService, EndpointService>();
        }

        [Test]
        public void partial_invoker_is_registered()
        {
            registeredTypeIs<IPartialInvoker, PartialInvoker>();
        }

        [Test]
        public void content_registry_is_registered()
        {
            registeredTypeIs<IContentRegistry, ContentRegistryCache>();
        }

        [Test]
        public void content_registry_cache_would_be_a_singleton()
        {
            ServiceRegistry.ShouldBeSingleton(typeof(ContentRegistryCache)).ShouldBeTrue();
        }

        [Test]
        public void combination_determination_service_is_registered()
        {
            registeredTypeIs<ICombinationDeterminationService, CombinationDeterminationService>();
        }

        [Test]
        public void asset_combination_cache_is_registered_as_a_singleton()
        {
            registeredTypeIs<IAssetCombinationCache, AssetCombinationCache>();
            ServiceRegistry.ShouldBeSingleton(typeof(AssetCombinationCache)).ShouldBeTrue();
        }

        [Test]
        public void asset_dependency_finder_should_be_registered_as_a_singleton()
        {
            registeredTypeIs<IAssetDependencyFinder, AssetDependencyFinderCache>();
            ServiceRegistry.ShouldBeSingleton(typeof(AssetDependencyFinderCache))
                .ShouldBeTrue();
        }

        [Test]
        public void asset_tag_plan_cache_is_registered_as_a_singleton()
        {
            registeredTypeIs<IAssetTagPlanCache, AssetTagPlanCache>();
            ServiceRegistry.ShouldBeSingleton(typeof (AssetTagPlanCache)).ShouldBeTrue();
        }

        [Test]
        public void asset_tag_planner_is_registered()
        {
            registeredTypeIs<IAssetTagPlanner, AssetTagPlanner>();
        }

        [Test]
        public void asset_requirements_are_registered()
        {
            registeredTypeIs<IAssetRequirements, AssetRequirements>();
        }

        [Test]
        public void asset_tag_builder_is_registered()
        {
            registeredTypeIs<IAssetTagBuilder, AssetTagBuilder>();
        }

        [Test]
        public void asset_pipeline_is_registered_as_both_IAssetPipeline_and_IAssetFileRegistration_as_the_same_instance()
        {
            var services = new FubuRegistry().BuildGraph().Services;
            var pipeline1 = services.DefaultServiceFor<IAssetPipeline>().Value.ShouldBeOfType<AssetPipeline>();
            var pipeline2 = services.DefaultServiceFor<IAssetFileRegistration>().Value.ShouldBeOfType<AssetPipeline>();

            pipeline1.ShouldNotBeNull();
            pipeline2.ShouldNotBeNull();

            pipeline1.ShouldBeTheSameAs(pipeline2);
        }

        [Test]
        public void by_default_the_missing_asset_handler_is_traceonle()
        {
            registeredTypeIs<IMissingAssetHandler, TraceOnlyMissingAssetHandler>();
        }

        [Test]
        public void script_graph_is_registered()
        {
            new FubuRegistry().BuildGraph().Services.DefaultServiceFor<AssetGraph>().Value.ShouldNotBeNull();
        }

        [Test]
        public void should_be_a_script_configuration_activator_registered_as_a_service()
        {
            new FubuRegistry().BuildGraph().Services.ServicesFor<IActivator>()
                .Any(x => x.Type == typeof(AssetGraphConfigurationActivator)).ShouldBeTrue();
        }

        [Test]
        public void asset_graph_and_pipeline_activators_are_registered_in_the_correct_order()
        {
            var activators = new FubuRegistry().BuildGraph().Services.ServicesFor<IActivator>().ToList();

            activators.Any(x => x.Type == typeof(AssetGraphConfigurationActivator)).ShouldBeTrue();
            activators.Any(x => x.Type == typeof(AssetPipelineBuilderActivator)).ShouldBeTrue();
            activators.Any(x => x.Type == typeof(AssetDeclarationVerificationActivator)).ShouldBeTrue();

            activators.RemoveAll(x => !x.Type.Namespace.Contains(typeof (AssetGraph).Namespace));

            activators[0].Type.ShouldEqual(typeof (AssetGraphConfigurationActivator));
            activators[1].Type.ShouldEqual(typeof(AssetPipelineBuilderActivator));
            activators[2].Type.ShouldEqual(typeof(AssetDeclarationVerificationActivator));
        }

        [Test]
        public void iscriptwriter_is_registered_to_the_basic_writer()
        {
            registeredTypeIs<IAssetTagWriter, AssetTagWriter>();
        }

        [Test]
        public void file_system_is_registered()
        {
            registeredTypeIs<IFileSystem, FileSystem>();
        }

        [Test]
        public void object_converter_is_registered()
        {
            registeredTypeIs<IObjectConverter, ObjectConverter>();
        }

        [Test]
        public void smart_request_is_registered_as_the_fubu_smart_request()
        {
            registeredTypeIs<ISmartRequest, FubuSmartRequest>();
        }

        [Test]
        public void request_history_cache_is_registered()
        {
            registeredTypeIs<IRequestHistoryCache, RequestHistoryCache>();
        }

        [Test]
        public void page_activation_rule_cache_is_registered()
        {
            registeredTypeIs<IPageActivationRules, PageActivationRuleCache>();
        }

        [Test]
        public void page_activator_is_registered()
        {
            registeredTypeIs<IPageActivator, PageActivator>();
        }

        [Test]
        public void chain_authorizor_is_registered()
        {
            registeredTypeIs<IChainAuthorizor, ChainAuthorizor>();
        }

        [Test]
        public void package_files_are_registered()
        {
            registeredTypeIs<IPackageFiles, PackageFilesCache>();
        }

        [Test]
        public void an_activator_for_PackageFileActivator_is_registered()
        {
            new FubuRegistry().BuildGraph().Services.ServicesFor<IActivator>()
                .Any(x => x.Type == typeof(PackageFileActivator)).ShouldBeTrue();
        }

        [Test]
        public void setter_binder_is_registered()
        {
            registeredTypeIs<ISetterBinder, SetterBinder>();
        }

        [Test]
        public void values_is_registered()
        {
            var services = new FubuRegistry().BuildGraph().Services;
            services.DefaultServiceFor(typeof (IValues<>)).Type.ShouldEqual(typeof (SimpleValues<>));
        }

        [Test]
        public void value_source_is_registered()
        {
            var services = new FubuRegistry().BuildGraph().Services;
            services.DefaultServiceFor(typeof(IValueSource<>)).Type.ShouldEqual(typeof(ValueSource<>));
        }
    }
}