using System.Linq;
using System.Web;
using FubuCore;
using HtmlTags;

namespace FubuMVC.Core.View
{
    public class ViewEnginesExtension : IFubuRegistryExtension
    {
        public void Configure(FubuRegistry registry)
        {
            registry.Configure(graph => {
                graph.Settings.Get<ViewEngines>().UseGraph(graph);
            });

            registry.AlterSettings<CommonViewNamespaces>(x =>
            {
                x.Add(typeof(VirtualPathUtility).Namespace); // System.Web
                x.AddForType<string>(); // System
                x.AddForType<FileSet>(); // FubuCore
                x.AddForType<ParallelQuery>(); // System.Linq
                x.AddForType<HtmlTag>(); // HtmlTags 
            });
        }
    }
}