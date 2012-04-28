using System.Collections;
using System.Collections.Generic;
using System.Net;
using FubuCore.Reflection;
using FubuMVC.Core.Ajax;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Headers;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.PathBased;
using FubuMVC.Core.Security;
using FubuMVC.Core.View.Attachment;
using HtmlTags;

namespace FubuMVC.Core
{
    public partial class FubuRegistry
    {
        private readonly IViewEngineRegistry _engineRegistry = new ViewEngineRegistry();
        private readonly IList<IViewBagConvention> _viewConventions = new List<IViewBagConvention>();

        private void setupDefaultConventionsAndPolicies()
        {
            // Add Behaviors First
            ApplyConvention(_behaviorAggregator);

            // Needs to go before routing conventions
            ApplyConvention<PartialOnlyConvention>();

            addConvention(graph => _routeResolver.ApplyToAll(graph));

            _systemPolicies.Add(new AttachAuthorizationPolicy());


            Policies.Add<ResourcePathRoutePolicy>();

            _systemPolicies.Add(new MissingRouteInputPolicy());

            ApplyConvention<ModifyChainAttributeConvention>();
            //Policies.Add<ModifyChainAttributeConvention>();
        }
    }
}