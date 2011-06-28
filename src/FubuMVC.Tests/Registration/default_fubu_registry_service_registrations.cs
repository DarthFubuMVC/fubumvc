using System.Linq;
using Bottles;
using FubuCore;
using FubuCore.Binding;
using FubuCore.Configuration;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Content;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security;
using FubuMVC.Core.SessionState;
using FubuMVC.Core.UI;
using FubuMVC.Core.UI.Scripts;
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
            registeredTypeIs<IOutputWriter, HttpResponseOutputWriter>();
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
            ServiceRegistry.ShouldBeSingleton(typeof(ContentRegistryCache), new ObjectDef(typeof(ContentRegistryCache))).ShouldBeTrue();
        }

        [Test]
        public void script_graph_is_registered()
        {
            new FubuRegistry().BuildGraph().Services.DefaultServiceFor<ScriptGraph>().Value.ShouldNotBeNull();
        }

        [Test]
        public void should_be_a_script_configuration_activator_registered_as_a_service()
        {
            new FubuRegistry().BuildGraph().Services.ServicesFor<IActivator>()
                .Any(x => x.Type == typeof(ScriptGraphConfigurationActivator)).ShouldBeTrue();
        }

        [Test]
        public void iscriptwriter_is_registered_to_the_basic_writer()
        {
            registeredTypeIs<IScriptTagWriter, BasicScriptTagWriter>();
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

    }
}