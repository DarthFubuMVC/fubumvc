using System.Collections.Generic;
using FubuCore;
using FubuCore.Binding;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Configuration;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security;
using FubuMVC.Core.SessionState;
using FubuMVC.Core.UI;
using FubuMVC.Core.UI.Configuration;
using FubuMVC.Core.UI.Diagnostics;
using FubuMVC.Core.UI.Security;
using FubuMVC.Core.UI.Tags;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;
using FubuMVC.Core.View.WebForms;
using FubuMVC.Core.Web.Security;

namespace FubuMVC.Core
{
    public partial class FubuRegistry
    {
        private void setupServices(BehaviorGraph graph)
        {

            graph.Services.AddService<ITypeResolver>(_typeResolver);
            graph.Services.AddService(new TypeDescriptorCache());

            graph.Services.SetServiceIfNone<IOutputWriter, HttpResponseOutputWriter>();
            graph.Services.SetServiceIfNone<IUrlRegistry, UrlRegistry>();
            graph.Services.SetServiceIfNone<IJsonWriter, JsonWriter>();
            graph.Services.SetServiceIfNone<ISecurityContext, WebSecurityContext>();
            graph.Services.SetServiceIfNone<IAuthenticationContext, WebAuthenticationContext>();
            graph.Services.SetServiceIfNone<IFlash, FlashProvider>();
            graph.Services.SetServiceIfNone<IRequestDataProvider, RequestDataProvider>();
            graph.Services.SetServiceIfNone<IWebFormRenderer, WebFormRenderer>();
            graph.Services.SetServiceIfNone<IWebFormsControlBuilder, WebFormsControlBuilder>();
            graph.Services.SetServiceIfNone<IFubuRequest, FubuRequest>();
            graph.Services.SetServiceIfNone<IValueConverterRegistry, ValueConverterRegistry>();
            graph.Services.SetServiceIfNone<IPartialFactory, PartialFactory>();
            graph.Services.SetServiceIfNone<IPartialRenderer, PartialRenderer>();
            graph.Services.SetServiceIfNone<IObjectResolver, ObjectResolver>();
            graph.Services.SetServiceIfNone<IRequestData, RequestData>();
            graph.Services.SetServiceIfNone<IViewActivator, NulloViewActivator>();
            graph.Services.SetServiceIfNone<IBindingContext, BindingContext>();
            graph.Services.SetServiceIfNone<ISettingsProvider, AppSettingsProvider>();
            graph.Services.SetServiceIfNone<IPropertyBinderCache, PropertyBinderCache>();
            graph.Services.SetServiceIfNone<IModelBinderCache, ModelBinderCache>();
            graph.Services.SetServiceIfNone<IDisplayFormatter, DisplayFormatter>();
            graph.Services.SetServiceIfNone<IChainResolver, ChainResolver>();
            graph.Services.SetServiceIfNone<IEndPointAuthorizorFactory, EndPointAuthorizorFactory>();
            graph.Services.SetServiceIfNone<IAuthorizationPreviewService, AuthorizationPreviewService>();
            graph.Services.SetServiceIfNone<IEndpointService, EndpointService>();
            graph.Services.SetServiceIfNone<IAuthorizationPolicyExecutor, AuthorizationPolicyExecutor>();

            graph.Services.SetServiceIfNone<ITypeDescriptorCache, TypeDescriptorCache>();
            graph.Services.SetServiceIfNone(_partialViewTypes);

            graph.Services.SetServiceIfNone<IStreamingData, StreamingData>();
            graph.Services.SetServiceIfNone<IJsonReader, JavaScriptJsonReader>();

            graph.Services.SetServiceIfNone<ISessionState, SimpleSessionState>();

            





            registerHtmlConventions(graph);
            registerAuthorizationServices(graph);
        }

        private void registerAuthorizationServices(BehaviorGraph graph)
        {
            graph.Services.SetServiceIfNone<IAuthorizationFailureHandler, DefaultAuthorizationFailureHandler>();
            graph.Services.SetServiceIfNone<IFieldAccessService, FieldAccessService>();

            if (graph.IsDiagnosticsEnabled())
            {
                graph.Services.SetServiceIfNone<IFieldAccessRightsExecutor, RecordingFieldAccessRightsExecutor>();
            }
            else
            {
                graph.Services.SetServiceIfNone<IFieldAccessRightsExecutor, FieldAccessRightsExecutor>();
            }
        }

        private void registerHtmlConventions(BehaviorGraph graph)
        {
            var library = new TagProfileLibrary();

            graph.Services.FindAllValues<HtmlConventionRegistry>()
                .Each(library.ImportRegistry);

            library.ImportRegistry(new DefaultHtmlConventions());

            library.Seal();

            graph.Services.ClearAll<HtmlConventionRegistry>();
            graph.Services.ReplaceService(library);
            graph.Services.SetServiceIfNone(typeof(ITagGenerator<>), typeof(TagGenerator<>));
            graph.Services.SetServiceIfNone<IElementNamingConvention, DefaultElementNamingConvention>();
        }
    }
}