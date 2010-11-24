using System;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Security;
using FubuMVC.Core.View.WebForms;
using HtmlTags;

namespace FubuMVC.Core
{
    public partial class FubuRegistry
    {
        private void setupDefaultConventionsAndPolicies()
        {
            // Default method filters
            Actions.IgnoreMethodsDeclaredBy<object>();
            Actions.IgnoreMethodsDeclaredBy<MarshalByRefObject>();
            Actions.IgnoreMethodsDeclaredBy<IDisposable>();

            // Add Behaviors First
            addConvention(graph => _behaviorMatcher.BuildBehaviors(_types, graph));
            addConvention(graph => _actionSourceMatcher.BuildBehaviors(_types, graph));
            addConvention(graph => _routeResolver.ApplyToAll(graph));
            _conventions.Add(new WrapWithAttributeConvention());

            _systemPolicies.Add(new AttachAuthorizationPolicy());

            Output.ToHtml.WhenCallMatches(x => x.Method.HasAttribute<HtmlEndpointAttribute>());
            Output.ToJson.WhenCallMatches(x => x.Method.HasAttribute<JsonEndpointAttribute>());
            Output.ToJson.WhenTheOutputModelIs<JsonMessage>();

            Output.To<RenderHtmlDocumentNode>().WhenTheOutputModelIs<HtmlDocument>();
            Output.To<RenderHtmlTagNode>().WhenTheOutputModelIs<HtmlTag>();

            Policies.Add<WebFormsEndpointPolicy>();
            Policies.Add<ContinuationHandlerConvention>();
	    Policies.Add<RedirectableHandlerConvention>();

            _systemPolicies.Add(new StringOutputPolicy());

            _conventions.Add(_viewAttacher);
            Policies.Add<JsonMessageInputConvention>();
            Policies.Add<UrlRegistryCategoryConvention>();
            Policies.Add<UrlForNewConvention>();

            ApplyConvention<AuthorizationAttributeConvention>();
        }
    }
}
