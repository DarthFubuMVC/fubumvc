namespace FubuMVC.Core.View
{
    public class ViewEnginesExtension : IFubuRegistryExtension
    {
        public void Configure(FubuRegistry registry)
        {
            registry.Configure(graph => {
                graph.Settings.Get<ViewEngines>().UseGraph(graph);
            });

            
        }
    }
}