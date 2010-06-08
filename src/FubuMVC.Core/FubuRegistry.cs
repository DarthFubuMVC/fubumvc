using System;
using System.Collections.Generic;
using FubuCore.Reflection;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;
using FubuMVC.Core.View.WebForms;
using HtmlTags;

namespace FubuMVC.Core
{

    // Register more and different types of actions
    public interface IActionSource
    {
        IEnumerable<ActionCall> FindActions(TypePool types);
    }



    // TODO:  blow up with a nice error if Route's have empty BehaviorChains


    public partial class FubuRegistry
    {
        private readonly BehaviorMatcher _behaviorMatcher = new BehaviorMatcher();

        private readonly List<IConfigurationAction> _conventions = new List<IConfigurationAction>();
        private readonly List<IConfigurationAction> _explicits = new List<IConfigurationAction>();
        private readonly List<RegistryImport> _imports = new List<RegistryImport>();
        private readonly List<IConfigurationAction> _policies = new List<IConfigurationAction>();
        private readonly ActionSourceMatcher _actionSourceMatcher = new ActionSourceMatcher();
        private readonly RouteDefinitionResolver _routeResolver = new RouteDefinitionResolver();
        private readonly List<IConfigurationAction> _systemPolicies = new List<IConfigurationAction>();
        private readonly TypePool _types = new TypePool();
        private readonly List<IUrlRegistrationConvention> _urlConventions = new List<IUrlRegistrationConvention>();

        private readonly UrlRegistry _urls = new UrlRegistry();
        private readonly IPartialViewTypeRegistry _partialViewTypes = new PartialViewTypeRegistry();
        private readonly ViewAttacher _viewAttacher;
        private IConfigurationObserver _observer;




        public FubuRegistry()
        {
            _observer = new NulloConfigurationObserver();

            _viewAttacher = new ViewAttacher(_types);

            // Default method filters
            Actions.IgnoreMethodsDeclaredBy<object>();
            Actions.IgnoreMethodsDeclaredBy<MarshalByRefObject>();
            Actions.IgnoreMethodsDeclaredBy<IDisposable>();

            // Add Behaviors First
            addConvention(graph => _behaviorMatcher.BuildBehaviors(_types, graph));
            addConvention(graph => _actionSourceMatcher.BuildBehaviors(_types, graph));
            addConvention(graph => _routeResolver.ApplyToAll(graph));


            Policies.Add<StringOutputPolicy>();
            Policies.Add<WebFormsEndpointPolicy>();
            Policies.Add<ContinuationHandlerConvention>();

            Output.ToHtml.WhenCallMatches(x => x.Method.HasAttribute<HtmlEndpointAttribute>());
            Output.ToJson.WhenCallMatches(x => x.Method.HasAttribute<JsonEndpointAttribute>());
            Output.ToJson.WhenTheOutputModelIs<JsonMessage>();

            Output.To<RenderHtmlDocumentNode>().WhenTheOutputModelIs<HtmlDocument>();
            Output.To<RenderHtmlTagNode>().WhenTheOutputModelIs<HtmlTag>();

            _conventions.Add(_viewAttacher);
            Policies.Add<JsonMessageInputConvention>();
            Policies.Add<UrlRegistryCategoryConvention>();

            _urlConventions.Add(new UrlForNewConvention());
        }

        public FubuRegistry(Action<FubuRegistry> configure)
            : this()
        {
            configure(this);
        }

        private void addConvention(Action<BehaviorGraph> action)
        {
            var convention = new LambdaConfigurationAction(action);
            _conventions.Add(convention);
        }

        private void addExplicit(Action<BehaviorGraph> action)
        {
            var convention = new LambdaConfigurationAction(action);
            _explicits.Add(convention);
        }

        public BehaviorGraph BuildGraph()
        {
            var graph = new BehaviorGraph(_observer);
            setupServices(graph);

            _conventions.Configure(graph);

            _imports.Each(x => x.ImportInto(graph));
            _explicits.Configure(graph);

            _policies.Configure(graph);
            _systemPolicies.Configure(graph);

            _urlConventions.Each(x => x.Configure(graph, _urls));

            registerUrls(graph);

            return graph;
        }

        private void registerUrls(BehaviorGraph graph)
        {
            var builder = new UrlRegistryBuilder(_urls);
            graph.VisitRoutes(builder);
        }
    }

    public interface IFubuRegistryExtension
    {
        void Configure(FubuRegistry registry);
    }
}