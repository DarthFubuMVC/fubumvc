using System;
using System.Net;
using FubuCore.Reflection;
using FubuMVC.Core.Ajax;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Http.Headers;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.PathBased;
using FubuMVC.Core.Security;
using HtmlTags;

namespace FubuMVC.Core
{
    public partial class FubuRegistry
    {
        private void setupDefaultConventionsAndPolicies()
        {
            _bagRunner.Apply(_viewAttacherConvention);

            // Add Behaviors First
            ApplyConvention(_behaviorAggregator);

            // Needs to go before routing conventions
            ApplyConvention<PartialOnlyConvention>();

            addConvention(graph => _routeResolver.ApplyToAll(graph));


            _systemPolicies.Add(new AttachAuthorizationPolicy());

            ApplyConvention<DictionaryOutputConvention>();

            Output.ToHtml.WhenCallMatches(x => x.Method.HasAttribute<HtmlEndpointAttribute>());
            Output.ToHtml
                .WhenCallMatches(x => x.Method.Name.ToLower().EndsWith("html") && x.OutputType() == typeof (string));
            
            Output.ToJson.WhenTheOutputModelIs<JsonMessage>();
            

            Output.To<RenderHtmlDocumentNode>().WhenTheOutputModelIs<HtmlDocument>();
            Output.To<RenderHtmlTagNode>().WhenTheOutputModelIs<HtmlTag>();

            Output.ToBehavior<RenderStatusCodeBehavior>().WhenTheOutputModelIs<HttpStatusCode>();

            Policies.Add<AjaxContinuationPolicy>();
            Policies.Add<ContinuationHandlerConvention>();
            Policies.Add<AsyncContinueWithHandlerConvention>();
            Policies.Add<HeaderWritingPolicy>();
            Policies.Add<ResourcePathRoutePolicy>();

            _systemPolicies.Add(new StringOutputPolicy());
            _systemPolicies.Add(new MissingRouteInputPolicy());

            _conventions.Add(_bagRunner);
            Policies.Add<JsonMessageInputConvention>();
            _systemPolicies.Add(_connegAttachmentPolicy);

            ApplyConvention<ModifyChainAttributeConvention>();
        }
    }
}