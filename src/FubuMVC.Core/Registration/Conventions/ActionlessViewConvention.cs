using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Attachment;

namespace FubuMVC.Core.Registration.Conventions
{
    public class ActionlessViewConvention : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            var attachedViews = graph.Behaviors
                .Where(x => x.HasOutput())
                .SelectMany(x => x.Output.Writers.OfType<ViewNode>())
                .Select(x => x.View).ToList();

            var unattached = graph.Settings.Get<ViewEngines>().Views.Views.Where(x => x.ViewModel != null && !attachedViews.Contains(x));

            unattached.Each(v =>
            {
                var chain = BehaviorChain.ForWriter(new ViewNode(v));
                chain.IsPartialOnly = true;

                graph.AddChain(chain);
            });
        }
    }
}