using System.Linq;
using FubuMVC.Core.Registration;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Attachment;

namespace FubuMVC.Core
{
    public class RegistryImport : IConfigurationAction
    {
        public string Prefix { get; set; }
        public FubuRegistry Registry { get; set; }

        public void ImportInto(IChainImporter graph, ViewBag views)
        {
            // TODO -- will want this to suck in the configuration log business somehow
            Registry.Compile();
            var childGraph = Registry.Configuration.BuildForImport(views);
            graph.Import(childGraph, b =>
            {
                b.PrependToUrl(Prefix);
                b.Origin = Registry.Name;
            });
        }

        public void Configure(BehaviorGraph graph)
        {
            ImportInto(graph, graph.Views);
            
            
            
        }
    }
}