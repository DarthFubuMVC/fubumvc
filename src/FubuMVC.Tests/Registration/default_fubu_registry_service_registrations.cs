using System.Linq;
using Bottles;
using FubuCore;
using FubuCore.Binding;
using FubuCore.Conversion;
using FubuCore.Formatting;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Http;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Resources.Media;
using FubuMVC.Core.Resources.Media.Projections;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security;
using FubuMVC.Core.SessionState;
using FubuMVC.Core.UI;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View.Activation;
using FubuMVC.Core.Web.Security;
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
        public void standin_current_http_request_is_used_as_the_default()
        {
            registeredTypeIs<ICurrentHttpRequest, StandInCurrentHttpRequest>();
        }

        [Test]
        public void BindingContext_is_registered()
        {
            registeredTypeIs<IBindingContext, BindingContext>();
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
        public void IRequestDataProvider_is_registered()
        {
            registeredTypeIs<IRequestDataProvider, RequestDataProvider>();
        }

        [Test]
        public void IRequestData_is_registered()
        {
            registeredTypeIs<IRequestData, RequestData>();
        }

        [Test]
        public void IRequestHeader_is_registered()
        {
            registeredTypeIs<IRequestHeaders, RequestHeaders>();
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
        public void an_activator_for_PackageFileActivator_is_registered()
        {
            new FubuRegistry().BuildGraph().Services.ServicesFor<IActivator>()
                .Any(x => x.Type == typeof (PackageFileActivator)).ShouldBeTrue();
        }

        [Test]
        public void an_activator_for_HtmlConventionActivator_is_registered()
        {
            new FubuRegistry().BuildGraph().Services.ServicesFor<IActivator>()
                .Any(x => x.Type == typeof(HtmlConventionsActivator)).ShouldBeTrue();
        }


        [Test]
        public void authorization_preview_service_is_registered()
        {
            registeredTypeIs<IAuthorizationPreviewService, AuthorizationPreviewService>();
        }

        [Test]
        public void chain_authorizor_is_registered()
        {
            registeredTypeIs<IChainAuthorizor, ChainAuthorizor>();
        }


        [Test]
        public void default_authorization_failure_handler_is_registered()
        {
            registeredTypeIs<IAuthorizationFailureHandler, DefaultAuthorizationFailureHandler>();
        }

        [Test]
        public void default_binding_logger_is_nullo()
        {
            registeredTypeIs<IBindingLogger, NulloBindingLogger>();
        }

        [Test]
        public void default_chain_resolver_is_registered()
        {
            ServiceRegistry.ShouldBeSingleton(typeof(ChainResolutionCache)).ShouldBeTrue();
            registeredTypeIs<IChainResolver, ChainResolutionCache>();
        }

        [Test]
        public void default_endpoint_factory_is_registered()
        {
            registeredTypeIs<IEndPointAuthorizorFactory, EndPointAuthorizorFactory>();
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
        public void endpoint_service_is_registered()
        {
            registeredTypeIs<IEndpointService, EndpointService>();
        }

        [Test]
        public void file_system_is_registered()
        {
            registeredTypeIs<IFileSystem, FileSystem>();
        }

        [Test]
        public void model_binder_cache_is_registered()
        {
            registeredTypeIs<IModelBinderCache, ModelBinderCache>();
        }

        [Test]
        public void object_converter_is_registered()
        {
            registeredTypeIs<IObjectConverter, ObjectConverter>();
        }

        [Test]
        public void package_files_are_registered()
        {
            registeredTypeIs<IPackageFiles, PackageFilesCache>();

            ServiceRegistry.ShouldBeSingleton(typeof (PackageFilesCache));
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
        public void partial_invoker_is_registered()
        {
            registeredTypeIs<IPartialInvoker, PartialInvoker>();
        }

        [Test]
        public void property_cache_is_registered()
        {
            registeredTypeIs<IPropertyBinderCache, PropertyBinderCache>();
        }

        [Test]
        public void request_history_cache_is_registered()
        {
            registeredTypeIs<IRequestHistoryCache, RequestHistoryCache>();
        }

        [Test]
        public void setter_binder_is_registered()
        {
            registeredTypeIs<ISetterBinder, SetterBinder>();
        }

        [Test]
        public void smart_request_is_registered_as_the_fubu_smart_request()
        {
            registeredTypeIs<ISmartRequest, FubuSmartRequest>();
        }

        [Test]
        public void url_registry_is_registered()
        {
            registeredTypeIs<IUrlRegistry, UrlRegistry>();
        }

        [Test]
        public void value_source_is_registered()
        {
            var services = new FubuRegistry().BuildGraph().Services;
            services.DefaultServiceFor(typeof (IValueSource<>)).Type.ShouldEqual(typeof (ValueSource<>));
        }

        [Test]
        public void values_is_registered()
        {
            var services = new FubuRegistry().BuildGraph().Services;
            services.DefaultServiceFor(typeof (IValues<>)).Type.ShouldEqual(typeof (SimpleValues<>));
        }



        [Test]
        public void projection_runner_is_registered()
        {
            registeredTypeIs<IProjectionRunner, ProjectionRunner>();
        }

        [Test]
        public void generic_projection_runner_is_registered()
        {
            var services = new FubuRegistry().BuildGraph().Services;
            services.DefaultServiceFor(typeof(IProjectionRunner<>)).Type.ShouldEqual(typeof(ProjectionRunner<>));
        }
    }
}