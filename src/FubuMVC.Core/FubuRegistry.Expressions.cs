using System;
using System.Linq;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Models;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.DSL;
using FubuMVC.Core.Registration.Nodes;
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
        public ActionCallFilterExpression JsonOutputIf { get { return output(call => call.OutputToJson()); } }

        public ActionCallFilterExpression HtmlOutputIf
        {
            get
            {
                return output(call => call.Append(new RenderTextNode<string>
                {
                    MimeType = MimeType.Html
                }));
            }
        }

        public ViewExpression Views { get { return new ViewExpression(_viewAttacher); } }

        public UrlCategoriesExpression Urls { get { return new UrlCategoriesExpression(convention => _urlConventions.Add(convention), _urls); } }
        public PoliciesExpression Policies { get { return new PoliciesExpression(_policies); } }

        public ModelsExpression Models { get { return new ModelsExpression(_explicits); } }
        public AppliesToExpression Applies { get { return new AppliesToExpression(_types); } }
        public ActionCallCandidateExpression Actions { get { return new ActionCallCandidateExpression(_behaviorMatcher, _types); } }

        public void Services(Action<IServiceRegistry> configure)
        {
            var action = new LambdaConfigurationAction(g => configure(g.Services));
            _explicits.Add(action);
        }

        public void Apply<TConvention>()
            where TConvention : IConfigurationAction, new()
        {
            Apply(new TConvention());
        }

        public void Apply<TConvention>(TConvention convention)
            where TConvention : IConfigurationAction, new()
        {
            _conventions.Add(convention);
        }

        private ActionCallFilterExpression output(Action<ActionCall> configure)
        {
            var modification = new ActionCallModification(configure);
            _conventions.Add(modification);

            modification.Filters.Excludes += call => call.HasOutputBehavior();

            return new ActionCallFilterExpression(modification.Filters);
        }

        public ActionCallFilterExpression OutputIs(Func<ActionCall, OutputNode> func)
        {
            return output(action =>
            {
                OutputNode node = func(action);
                action.Append(node);
            });
        }

        public ActionCallFilterExpression OutputIs<T>() where T : OutputNode, new()
        {
            return output(action => action.Append(new T()));
        }


        public ChainedBehaviorExpression HomeIs()
        {
            return Route(string.Empty);
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
                Import<DiagnosticsRegistry>(string.Empty);
                Modify<DiagnosticsPackage>();
                _systemPolicies.Add(new DiagnosticBehaviorPrepender());
            }
        }

        private void setupServices(BehaviorGraph graph)
        {
            graph.Services.AddService<IUrlRegistry>(_urls);
            graph.Services.AddService<IUrlRegistration>(_urls);
            graph.Services.AddService<ITypeDescriptorRegistry>(new TypeDescriptorRegistry());

            graph.Services.SetServiceIfNone<IOutputWriter, HttpResponseOutputWriter>();
            graph.Services.SetServiceIfNone<ISecurityContext, WebSecurityContext>();
            graph.Services.SetServiceIfNone<IAuthenticationContext, WebAuthenticationContext>();
            graph.Services.SetServiceIfNone<IFlash, FlashProvider>();
            graph.Services.SetServiceIfNone<IRequestDataProvider, RequestDataProvider>();
            graph.Services.SetServiceIfNone<IWebFormRenderer, WebFormRenderer>();
            graph.Services.SetServiceIfNone<IWebFormsControlBuilder, WebFormsControlBuilder>();
            graph.Services.SetServiceIfNone<IFubuRequest, FubuRequest>();
            graph.Services.SetServiceIfNone<IValueConverterRegistry, ValueConverterRegistry>();
            graph.Services.SetServiceIfNone<IPartialFactory, PartialFactory>();
            graph.Services.SetServiceIfNone<IObjectResolver, ObjectResolver>();
            graph.Services.SetServiceIfNone<IRequestData, RequestData>();
            graph.Services.SetServiceIfNone<IViewActivator, NulloViewActivator>();
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