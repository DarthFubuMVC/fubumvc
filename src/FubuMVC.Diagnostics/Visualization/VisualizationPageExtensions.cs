using System.Collections.Generic;
using System.Text;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.UI.Bootstrap;
using FubuMVC.Core.View;
using HtmlTags;

namespace FubuMVC.Diagnostics.Visualization
{
    public static class VisualizationPageExtensions
    {
        public static HtmlTag Visualize(this IFubuPage page, BehaviorNode node)
        {
            var visualizer = page.Get<Visualizer>(); // TODO -- change this
            var model = visualizer.ToVisualizationSubject(node);

            var html =
                page.CollapsiblePartialFor<BehaviorNodeViewModel>(model).Title(model.Description.Title).ToString();

            return new HtmlTag("a", a => {
                a.Attr("name", node.UniqueId.ToString());
                a.Next = new LiteralTag(html);
            });
        }

        public static string Visualize(this IFubuPage page, IEnumerable<BehaviorNode> nodes)
        {
            var builder = new StringBuilder();

            nodes.Each(x => builder.Append(page.Visualize(x)));

            return builder.ToString();
        }
    }
}