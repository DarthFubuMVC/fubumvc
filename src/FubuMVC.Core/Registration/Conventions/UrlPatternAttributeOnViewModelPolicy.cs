using System;
using System.ComponentModel;
using System.Linq;
using System.Collections.Generic;
using FubuCore.Descriptions;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Routes;

namespace FubuMVC.Core.Registration.Conventions
{
    [Title("[UrlPattern] on actionless view models")]
    [Description("Applies a route matching the [UrlPattern] attribute on the Resource Type (View Model) of a BehaviorChain.  Mostly used to add a Route to an 'actionless view'")]
    public class UrlPatternAttributeOnViewModelPolicy : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Behaviors.Where(chain => chain.Route == null && chain.HasResourceType()).Each(chain =>
            {
                chain.ResourceType().ForAttribute<UrlPatternAttribute>(att =>
                {
                    chain.IsPartialOnly = false;
                    chain.Route = RouteBuilder.Build(chain.ResourceType(), att.Pattern);
                });
            });
        }
    }
}