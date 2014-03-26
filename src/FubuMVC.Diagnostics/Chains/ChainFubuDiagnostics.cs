using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Descriptions;
using FubuMVC.Core;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.UI;
using FubuMVC.Core.UI.Bootstrap.Tags;
using FubuMVC.Core.Urls;
using FubuMVC.Diagnostics.Endpoints;
using FubuMVC.Diagnostics.Visualization;
using HtmlTags;

namespace FubuMVC.Diagnostics.Chains
{
    public class ChainFubuDiagnostics
    {
        private readonly FubuHtmlDocument _document;
        private readonly BehaviorGraph _graph;
        private readonly IUrlRegistry _urls;

        public ChainFubuDiagnostics(IUrlRegistry urls, BehaviorGraph graph, FubuHtmlDocument document)
        {
            _urls = urls;
            _graph = graph;
            _document = document;
        }

        [System.ComponentModel.Description("Chain Details")]
        public ChainVisualization get_chain_details_Id(ChainDetailsRequest request)
        {
            writeAssets();

            var chain = _graph.Behaviors.FirstOrDefault(x => x.UniqueId == request.Id);
            if (chain == null)
            {
                return new ChainVisualization
                {
                    RedirectTo = FubuContinuation.RedirectTo<ChainFubuDiagnostics>(x => x.get_chain_missing())
                };
            }

            var report = RouteReport.ForChain(chain, _urls);

            return new ChainVisualization{
                Chain = chain,
                Details = buildDetails(report),
                Report = report,
                BehaviorVisualization = new LiteralTag(_document.Visualize(chain.NonDiagnosticNodes()))
            };
        }

        private HtmlTag buildDocument(Guid chainId, Action<DetailsTableTag, BehaviorChain, RouteReport> action)
        {
            writeAssets();

            var chain = _graph.Behaviors.FirstOrDefault(x => x.UniqueId == chainId);

            if (chain == null)
            {
                return new HtmlTag("div").Text("This route cannot be found");
            }

            var top = _document.Push("div");

            var report = RouteReport.ForChain(chain, _urls);
            var details = buildDetails(report);

            action(details, chain, report);

            return top;
        }

        [FubuPartial]
        public HtmlTag get_chain_missing()
        {
            return new HtmlTag("p", p =>
            {
                p.Add("span").Text("The requested BehaviorChain cannot be found.  ");
                p.Add("a").Text("Return to the Request Explorer").Attr("href", _urls.UrlFor<EndpointExplorerModel>());
            });
        }


        public HtmlTag get_chain_Id(ChainRequest request)
        {
            return buildDocument(request.Id, (details, chain, report) =>
            {
                var behaviorsTag = createBehaviorList(chain);
                details.AddDetail("Behaviors", behaviorsTag);
            });
        }

        private void writeAssets()
        {
            _document.Asset("twitterbootstrap");
            _document.Asset("diagnostics/bootstrap.overrides.css");
        }

        private HtmlTag createBehaviorList(BehaviorChain chain)
        {
            var div = new HtmlTag("div").Id("chain-summary");

            var level = 0;
            chain.NonDiagnosticNodes().Each(node =>
            {
                var description = Description.For(node);

                var child = div.Add("div").AddClass("node-title");

                if (level > 0)
                {
                    var image = _document.Image("arrow-turn-000-left-icon.png");
                    image.Style("padding-left", (level*5) + "px");

                    child.Append(image);
                }

                child.Add("span").Text(description.Title);

                level++;
            });

            return div;
        }


        private DetailsTableTag buildDetails(RouteReport report)
        {
            var builder = new DetailTableBuilder(_document);
            builder.AddDetail("Route", report.Route);
            builder.AddDetail("Http Verbs", report.Constraints);

            builder.AddDetail("Url Category", report.UrlCategory);
            builder.AddDetail("Origin", report.Origin);

            builder.AddDetail("Input Type", report.InputModel);
            builder.AddDetail("Resource Type", report.ResourceType);

            builder.AddDetail("Accepts", report.Accepts);

            builder.AddDetail("Content Type", report.ContentType);

            return builder.DetailTag;
        }
    }

    public class BehaviorOutlineTag : OutlineTag
    {
        public BehaviorOutlineTag(BehaviorChain chain)
        {
            AddHeader("Behaviors");

            if (chain is RoutedChain)
            {
                AddNode("Route", ChainVisualization.RouteDescId);
            }

            chain.NonDiagnosticNodes().Each(x =>
            {
                var description = Description.For(x);
                AddNode(description.Title, x.UniqueId.ToString());
            });
        }
    }
}