using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FubuMVC.Core.Registration;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Model;
using FubuMVC.Razor.RazorModel;

namespace FubuMVC.Razor
{
    public class RazorViewFacility : IViewFacility
    {
        public Task<IEnumerable<ITemplateFile>> FindViews(BehaviorGraph graph)
        {
            return Task.Factory.StartNew(() => findViews(graph));
        }

        private IEnumerable<ITemplateFile> findViews(BehaviorGraph graph)
        {
            var razorSettings = graph.Settings.Get<RazorEngineSettings>();
            var namespaces = graph.Settings.Get<CommonViewNamespaces>();

            var factory = new TemplateFactoryCache(namespaces, razorSettings, new TemplateCompiler(),
                new RazorTemplateGenerator());

            return graph.Files.FindFiles(razorSettings.Search)
                .Select(file => new RazorTemplate(file, factory));
        } 

    }
}