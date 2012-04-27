using System.Collections.Generic;
using System.Linq;
using HtmlTags;
using FubuCore;

namespace FubuMVC.Core.Registration.Conventions
{
    public class HtmlTagOutputPolicy : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Behaviors
                .Where(x => x.ResourceType() != null && x.ResourceType().CanBeCastTo<HtmlTag>() || x.ResourceType().CanBeCastTo<HtmlDocument>())
                .Each(x => x.Output.AddHtml());
        }
    }
}