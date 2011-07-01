using System;
using System.Collections.Generic;
using System.Reflection;
using Bottles;
using FubuCore;
using FubuCore.Binding;
using FubuCore.Configuration;
using FubuCore.Reflection;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Conneg;
using FubuMVC.Core.Content;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.DSL;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Routing;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security;
using FubuMVC.Core.SessionState;
using FubuMVC.Core.UI;
using FubuMVC.Core.UI.Configuration;
using FubuMVC.Core.UI.Diagnostics;
using FubuMVC.Core.UI.Scripts;
using FubuMVC.Core.UI.Security;
using FubuMVC.Core.UI.Tags;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View.Activation;
using FubuMVC.Core.Web.Security;
using IPackageFiles = FubuMVC.Core.Packaging.IPackageFiles;

namespace FubuMVC.Core
{
    public interface IFubuRegistry
    {
        RouteConventionExpression Routes { get; }
        OutputDeterminationExpression Output { get; }
        ViewExpression Views { get; }
        PoliciesExpression Policies { get; }
        ModelsExpression Models { get; }
        AppliesToExpression Applies { get; }
        ActionCallCandidateExpression Actions { get; }
        MediaExpression Media { get; }
        DiagnosticLevel DiagnosticLevel { get; }
        string Name { get; }
        void ResolveTypes(Action<TypeResolver> configure);
        void UsingObserver(IConfigurationObserver observer);
        void Services(Action<IServiceRegistry> configure);

        void ApplyConvention<TConvention>()
            where TConvention : IConfigurationAction, new();

        void ApplyConvention<TConvention>(TConvention convention)
            where TConvention : IConfigurationAction;

        ChainedBehaviorExpression Route(string pattern);
        void Import<T>(string prefix) where T : FubuRegistry, new();
        void Import(FubuRegistry registry, string prefix);
        void IncludeDiagnostics(bool shouldInclude);
        void IncludeDiagnostics(Action<IDiagnosticsConfigurationExpression> configure);
        

        /// <summary>
        ///   This allows you to drop down to direct manipulation of the BehaviorGraph
        ///   produced by this FubuRegistry
        /// </summary>
        /// <param name = "alteration"></param>
        void Configure(Action<BehaviorGraph> alteration);

        void HtmlConvention<T>() where T : HtmlConventionRegistry, new();
        void HtmlConvention(HtmlConventionRegistry conventions);
        void HtmlConvention(Action<HtmlConventionRegistry> configure);
        void StringConversions<T>() where T : DisplayConversionRegistry, new();
        void StringConversions(Action<DisplayConversionRegistry> configure);
        void ActionCallProvider(Func<Type, MethodInfo, ActionCall> actionCallProvider);
        BehaviorGraph BuildGraph();
    }

    public interface IDiagnosticsConfigurationExpression
    {
        void LimitRecordingTo(int nrRequests);
        void ExcludeRequests(IRequestHistoryCacheFilter filter);
    }

    public static class DiagnosticsConfigurationExtensions
    {
        public static void ApplyFilter<TFilter>(this IDiagnosticsConfigurationExpression config)
            where TFilter : IRequestHistoryCacheFilter, new()
        {
            config.ExcludeRequests(new TFilter());
        }

        public static void ExcludeRequests(this IDiagnosticsConfigurationExpression config, Func<CurrentRequest, bool> shouldExclude)
        {
            config.ExcludeRequests(new LambdaRequestHistoryCacheFilter(shouldExclude));
        }
    }

    public class DiagnosticsConfiguration
    {
        public int MaxRequests { get; set; }
    }

    public class DiagnosticsConfigurationExpression : IDiagnosticsConfigurationExpression
    {
        private readonly IList<IRequestHistoryCacheFilter> _filters;

        public DiagnosticsConfigurationExpression(IList<IRequestHistoryCacheFilter> filters)
        {
            _filters = filters;
        }

        public int MaxRequests { get; private set; }

        public void LimitRecordingTo(int nrRequests)
        {
            MaxRequests = nrRequests;
        }

        public void ExcludeRequests(IRequestHistoryCacheFilter filter)
        {
            _filters.Fill(filter);
        }
    }

    public partial class FubuRegistry
    {
        private void setupServices(BehaviorGraph graph)
        {

            graph.Services.SetServiceIfNone<ITypeResolver, TypeResolver>();
            graph.Services.AddService(new TypeDescriptorCache());

            graph.Services.SetServiceIfNone<IOutputWriter, HttpResponseOutputWriter>();
            graph.Services.SetServiceIfNone<IUrlRegistry, UrlRegistry>();
            graph.Services.SetServiceIfNone<IUrlTemplatePattern, NulloUrlTemplate>();
            graph.Services.SetServiceIfNone<IJsonWriter, JsonWriter>();
            graph.Services.SetServiceIfNone<ISecurityContext, WebSecurityContext>();
            graph.Services.SetServiceIfNone<IAuthenticationContext, WebAuthenticationContext>();
            graph.Services.SetServiceIfNone<IFlash, FlashProvider>();
            graph.Services.SetServiceIfNone<IRequestDataProvider, RequestDataProvider>();

            graph.Services.SetServiceIfNone<IFubuRequest, FubuRequest>();
            graph.Services.SetServiceIfNone<IValueConverterRegistry, ValueConverterRegistry>();
            graph.Services.SetServiceIfNone<IPartialFactory, PartialFactory>();

            graph.Services.SetServiceIfNone<IObjectResolver, ObjectResolver>();
            graph.Services.SetServiceIfNone<IRequestData, RequestData>();
            graph.Services.SetServiceIfNone<IBindingContext, BindingContext>();
            graph.Services.SetServiceIfNone<IPropertyBinderCache, PropertyBinderCache>();
            graph.Services.SetServiceIfNone<IModelBinderCache, ModelBinderCache>();
            graph.Services.SetServiceIfNone<IDisplayFormatter, DisplayFormatter>();
            graph.Services.SetServiceIfNone<IChainResolver, ChainResolver>();
            graph.Services.SetServiceIfNone<IEndPointAuthorizorFactory, EndPointAuthorizorFactory>();
            graph.Services.SetServiceIfNone<IAuthorizationPreviewService, AuthorizationPreviewService>();
            graph.Services.SetServiceIfNone<IEndpointService, EndpointService>();
            graph.Services.SetServiceIfNone<IAuthorizationPolicyExecutor, AuthorizationPolicyExecutor>();
            graph.Services.SetServiceIfNone<IChainAuthorizor, ChainAuthorizor>();


            graph.Services.SetServiceIfNone<ICollectionTypeProvider, DefaultCollectionTypeProvider>();

            graph.Services.SetServiceIfNone<ITypeDescriptorCache, TypeDescriptorCache>();


            graph.Services.SetServiceIfNone<IStreamingData, StreamingData>();
            graph.Services.SetServiceIfNone<IJsonReader, JavaScriptJsonReader>();

            graph.Services.SetServiceIfNone<ISessionState, SimpleSessionState>();

            graph.Services.SetServiceIfNone<IPartialInvoker, PartialInvoker>();

            graph.Services.SetServiceIfNone<IContentRegistry, ContentRegistryCache>();

            graph.Services.SetServiceIfNone(new ScriptGraph());

            graph.Services.SetServiceIfNone<IScriptTagWriter, BasicScriptTagWriter>();
            graph.Services.SetServiceIfNone<ICssLinkTagWriter, CssLinkTagWriter>();

            graph.Services.SetServiceIfNone<IFileSystem, FileSystem>();
            
            graph.Services.SetServiceIfNone<IRoutePolicy, StandardRoutePolicy>();

            graph.Services.SetServiceIfNone<IObjectConverter, ObjectConverter>();

            graph.Services.SetServiceIfNone<ISmartRequest, FubuSmartRequest>();

            graph.Services.SetServiceIfNone(new DiagnosticsIndicator());


            graph.Services.AddService<IFormatter, JsonFormatter>();
            graph.Services.AddService<IFormatter, XmlFormatter>();
            graph.Services.SetServiceIfNone(typeof (IMediaProcessor<>), typeof (MediaProcessor<>));

            graph.Services.SetServiceIfNone<IRequestHistoryCache, RequestHistoryCache>();


            graph.Services.SetServiceIfNone<IPageActivationRules, PageActivationRuleCache>();
            graph.Services.SetServiceIfNone<IPageActivator, PageActivator>();

            graph.Services.SetServiceIfNone<IPackageFiles, PackageFilesCache>();
            graph.Services.AddService<IActivator>(typeof (PackageFileActivator));

            registerActivators(graph);
            registerHtmlConventions(graph);
            registerAuthorizationServices(graph);
        }

        private void registerActivators(BehaviorGraph graph)
        {
            graph.Services.FillType(typeof (IActivator), typeof (ScriptGraphConfigurationActivator));
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