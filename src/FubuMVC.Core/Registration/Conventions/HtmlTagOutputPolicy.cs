using System.Collections.Generic;
using System.Linq;
using HtmlTags;

namespace FubuMVC.Core.Registration.Conventions
{
    public class HtmlTagOutputPolicy : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Behaviors
                .Where(x => x.ActionOutputType() == typeof (HtmlTag) || x.ActionOutputType() == typeof (HtmlDocument))
                .Each(x => x.Output.AddHtml());
        }
    }
}