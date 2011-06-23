using System;
using System.Net;
using FubuCore.Reflection;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Security;
using HtmlTags;

namespace FubuMVC.Core
{
    public partial class FubuRegistry : IFubuRegistry
    {
        private void setupDefaultConventionsAndPolicies()
        {
            _bagRunner.Apply(_viewAttacherConvention);

            // Add Behaviors First
            addConvention(graph => _behaviorMatcher.BuildBehaviors(_types, graph));
            addConvention(graph => _actionSourceMatcher.BuildBehaviors(_types, graph));

            // need to find partials *before* the route resolver hits
            ApplyConvention<PartialRequestConvention>();

            addConvention(graph => _routeResolver.ApplyToAll(graph));
            _conventions.Add(new WrapWithAttributeConvention());


            _systemPolicies.Add(new AttachAuthorizationPolicy());

            Output.ToHtml.WhenCallMatches(x => x.Method.HasAttribute<HtmlEndpointAttribute>());
            Output.ToHtml
                .WhenCallMatches(x => x.Method.Name.ToLower().EndsWith("html") && x.OutputType() == typeof (string));
            
            Output.ToJson.WhenCallMatches(x => x.Method.HasAttribute<JsonEndpointAttribute>());
            Output.ToJson.WhenTheOutputModelIs<JsonMessage>();

            Output.To<RenderHtmlDocumentNode>().WhenTheOutputModelIs<HtmlDocument>();
            Output.To<RenderHtmlTagNode>().WhenTheOutputModelIs<HtmlTag>();

            Output.ToBehavior<RenderStatusCodeBehavior>().WhenTheOutputModelIs<HttpStatusCode>();

            
            Policies.Add<ContinuationHandlerConvention>();

            _systemPolicies.Add(new StringOutputPolicy());
            _systemPolicies.Add(new MissingRouteInputPolicy());

            _conventions.Add(_bagRunner);
            Policies.Add<JsonMessageInputConvention>();
            Policies.Add<UrlRegistryCategoryConvention>();
            Policies.Add<UrlForNewConvention>();

            ApplyConvention<AuthorizationAttributeConvention>();
        }
    }
}