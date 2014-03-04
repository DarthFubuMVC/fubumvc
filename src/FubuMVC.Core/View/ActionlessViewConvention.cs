using FubuMVC.Core.Registration;

namespace FubuMVC.Core.View
{
    [Policy]
    public class ActionlessViewConvention : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
//            var attachedViews = graph.Behaviors
//                .Where(x => x.HasOutput())
//                .SelectMany(x => x.Output.Writers.OfType<ViewNode>())
//                .Select(x => x.View).ToList();
//
//            var unattached = graph.Settings.Get<ViewEngines>().Views.Views.Where(x => x.ViewModel != null && !attachedViews.Contains(x));
//
//            unattached.Each(v =>
//            {
//                var chain = BehaviorChain.ForWriter(new ViewNode(v));
//                chain.IsPartialOnly = true;
//                chain.UrlCategory.Category = Categories.VIEW;
//
//                graph.AddChain(chain);
//            });
        }
    }
}