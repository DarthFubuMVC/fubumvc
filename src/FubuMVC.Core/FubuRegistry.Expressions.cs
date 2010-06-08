using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore;
using FubuCore.Binding;
using FubuCore.Reflection;
using FubuMVC.Core.Configuration;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.DSL;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security;
using FubuMVC.Core.SessionState;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;
using FubuMVC.Core.View.WebForms;
using FubuMVC.Core.Web.Security;

namespace FubuMVC.Core
{
    public interface IRegistryModification
    {
        void Modify(FubuRegistry registry);
    }

    public partial class FubuRegistry
    {
        public RouteConventionExpression Routes { get { return new RouteConventionExpression(_routeResolver, this); } }
        public OutputDeterminationExpression Output { get { return new OutputDeterminationExpression(this); } }
        public ViewExpression Views { get { return new ViewExpression(_viewAttacher); } }

        public UrlRegistryExpression UrlRegistry { get { return new UrlRegistryExpression(convention => _urlConventions.Add(convention), _urls); } }
        public PoliciesExpression Policies { get { return new PoliciesExpression(_policies); } }

        public ModelsExpression Models { get { return new ModelsExpression(addExplicit); } }
        public AppliesToExpression Applies { get { return new AppliesToExpression(_types); } }
        public ActionCallCandidateExpression Actions { get { return new ActionCallCandidateExpression(_behaviorMatcher, _types, _actionSourceMatcher); } }

        public void UsingObserver(IConfigurationObserver observer)
        {
            _observer = observer;
        }

        public void Services(Action<IServiceRegistry> configure)
        {
            var action = new LambdaConfigurationAction(g => configure(g.Services));
            _explicits.Add(action);
        }

        public void ApplyConvention<TConvention>()
            where TConvention : IConfigurationAction, new()
        {
            ApplyConvention(new TConvention());
        }

        public void ApplyConvention<TConvention>(TConvention convention)
            where TConvention : IConfigurationAction
        {
            _conventions.Add(convention);
        }

        public void HomeIs<TController>(Expression<Action<TController>> controllerAction)
        {
            MethodInfo method = ReflectionHelper.GetMethod(controllerAction);
            _routeResolver.RegisterUrlPolicy(new DefaultRouteMethodBasedUrlPolicy(method));
        }

        public void HomeIs<TModel>()
        {
            _routeResolver.RegisterUrlPolicy(new DefaultRouteInputTypeBasedUrlPolicy(typeof (TModel)));
        }

        public ChainedBehaviorExpression Route(string pattern)
        {
            var expression = new ExplicitRouteConfiguration(pattern);
            _explicits.Add(expression);

            return expression.Chain();
        }

        public ChainedBehaviorExpression Route<T>(string pattern)
        {
            // TODO:  Throw exception in the chained expression if the input types
            // do not match
            var expression = new ExplicitRouteConfiguration<T>(pattern);
            _explicits.Add(expression);

            return expression.Chain();
        }

        public void Import<T>(string prefix) where T : FubuRegistry, new()
        {
            if (_imports.Any(x => x.Registry is T)) return;

            Import(new T(), prefix);
        }

        public void Modify<T>() where T : IRegistryModification, new()
        {
            new T().Modify(this);
        }

        public void Import(FubuRegistry registry, string prefix)
        {
            _imports.Add(new RegistryImport
            {
                Prefix = prefix,
                Registry = registry
            });
        }

        public void IncludeDiagnostics(bool shouldInclude)
        {
            if (shouldInclude)
            {
                UsingObserver(new RecordingConfigurationObserver());
                Import<DiagnosticsRegistry>(string.Empty);
                Modify<DiagnosticsPackage>();
                _systemPolicies.Add(new DiagnosticBehaviorPrepender());
            }
            else
            {
                Actions
                    .ExcludeTypes(t => t.HasAttribute<DiagnosticsActionAttribute>())
                    .ExcludeMethods(call => call.Method.HasAttribute<DiagnosticsActionAttribute>());
            }
        }

        public void RegisterPartials(Action<IPartialViewTypeRegistrationExpression> registration)
        {
            var expression = new PartialViewTypeRegistrationExpression(_partialViewTypes);
            registration(expression);
        }

        private void setupServices(BehaviorGraph graph)
        {
            graph.Services.AddService<IUrlRegistry>(_urls);
            graph.Services.AddService<IUrlRegistration>(_urls);
            graph.Services.AddService(new TypeDescriptorCache());

            graph.Services.SetServiceIfNone<IOutputWriter, HttpResponseOutputWriter>();
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

            graph.Services.SetServiceIfNone<ITypeDescriptorCache, TypeDescriptorCache>();
            graph.Services.SetServiceIfNone(_partialViewTypes);

            graph.Services.SetServiceIfNone<IStreamingData, StreamingData>();
            graph.Services.SetServiceIfNone<IJsonReader, JavaScriptJsonReader>();

            graph.Services.SetServiceIfNone<ISessionState, SimpleSessionState>();
        }

        #region Nested type: RegistryImport

        public class RegistryImport
        {
            public string Prefix { get; set; }
            public FubuRegistry Registry { get; set; }

            public void ImportInto(BehaviorGraph graph)
            {
                graph.Import(Registry.BuildGraph(), Prefix);
            }
        }

        #endregion
    }
}