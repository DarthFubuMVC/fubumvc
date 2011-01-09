using System.Linq;
using FubuCore;
using FubuCore.Binding;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Configuration;
using FubuMVC.Core.Content;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security;
using FubuMVC.Core.SessionState;
using FubuMVC.Core.UI;
using FubuMVC.Core.UI.Scripts;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;
using FubuMVC.Core.View.WebForms;
using FubuMVC.Core.Web.Security;
using NUnit.Framework;

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
        public void IWebFormsControlBuilder_is_registered()
        {
            registeredTypeIs<IWebFormsControlBuilder, WebFormsControlBuilder>();
        }

        [Test]
        public void IWebRenderer_is_registered()
        {
            registeredTypeIs<IWebFormRenderer, WebFormRenderer>();
        }

        [Test]
        public void ValueConverterRegistry_is_registered()
        {
            registeredTypeIs<IValueConverterRegistry, ValueConverterRegistry>();
        }

        [Test]
        public void NulloViewActivator_is_registered()
        {
            registeredTypeIs<IViewActivator, NulloViewActivator>();
        }

        [Test]
        public void BindingContext_is_registered()
        {
            registeredTypeIs<IBindingContext, BindingContext>();
        }

        [Test]
        public void AppSettingsProvider_is_registered()
        {
            registeredTypeIs<ISettingsProvider, AppSettingsProvider>();
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
    }
}