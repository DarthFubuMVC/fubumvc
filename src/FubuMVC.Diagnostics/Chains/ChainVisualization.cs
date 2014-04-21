using System;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Descriptions;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.UI.Bootstrap.Collapsibles;
using FubuMVC.Core.UI.Bootstrap.Tags;
using FubuMVC.Diagnostics.Endpoints;
using FubuMVC.Diagnostics.Visualization;
using HtmlTags;
using System.Linq;

namespace FubuMVC.Diagnostics.Chains
{
    public class ChainVisualization : IRedirectable
    {
        public static readonly string RouteDescId = "route-desc";

        public RouteReport Report { get; set; }
        public BehaviorChain Chain { get; set; }

        public LiteralTag BehaviorVisualization { get; set; }

        public string Title
        {
            get {
                return Chain.Title();
            }
        }



        public DetailsTableTag Details { get; set; }
        public FubuContinuation RedirectTo { get; set; }


        public BehaviorOutlineTag BehaviorsOutline
        {
            get
            {
                return new BehaviorOutlineTag(Chain);
            }
        }

        public HtmlTag RouteTag
        {
            get
            {
                var routed = Chain as RoutedChain;

                if (routed == null)
                {
                    return new HtmlTag("div").Render(false);
                }

                var description = Description.For(routed.Route);
                var collapsedTag = new CollapsibleTag(RouteDescId, "Route");
                collapsedTag.PrependAnchor();
                collapsedTag.AppendContent(new DescriptionBodyTag(description));

                return collapsedTag;
            }
        }
    }
}