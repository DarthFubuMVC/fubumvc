using FubuCore.Binding;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Configuration;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security;
using FubuMVC.Core.SessionState;
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
    }
}