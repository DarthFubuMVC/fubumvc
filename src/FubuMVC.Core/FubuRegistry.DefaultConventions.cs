using System;
using System.Net;
using FubuCore.Reflection;
using FubuMVC.Core.Ajax;
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

            // Default action sources
            _actionSources.Add(_behaviorMatcher);

            // Add Behaviors First
            ApplyConvention(_behaviorAggregator);


            addConvention(graph => _routeResolver.ApplyToAll(graph));


            _systemPolicies.Add(new AttachAuthorizationPolicy());

            Output.ToHtml.WhenCallMatches(x => x.Method.HasAttribute<HtmlEndpointAttribute>());
            Output.ToHtml
                .WhenCallMatches(x => x.Method.Name.ToLower().EndsWith("html") && x.OutputType() == typeof (string));
            
            Output.ToJson.WhenTheOutputModelIs<JsonMessage>();

            Output.To<RenderHtmlDocumentNode>().WhenTheOutputModelIs<HtmlDocument>();
            Output.To<RenderHtmlTagNode>().WhenTheOutputModelIs<HtmlTag>();

            Output.ToBehavior<RenderStatusCodeBehavior>().WhenTheOutputModelIs<HttpStatusCode>();

            Policies.Add<AjaxContinuationPolicy>();
            Policies.Add<ContinuationHandlerConvention>();

            _systemPolicies.Add(new StringOutputPolicy());
            _systemPolicies.Add(new MissingRouteInputPolicy());

            _conventions.Add(_bagRunner);
            Policies.Add<JsonMessageInputConvention>();
            _systemPolicies.Add(_connegAttachmentPolicy);

            ApplyConvention<ModifyChainAttributeConvention>();
        }
    }
}