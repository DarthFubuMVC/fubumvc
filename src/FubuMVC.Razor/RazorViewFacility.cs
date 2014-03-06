using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FubuMVC.Core.Registration;
using FubuMVC.Core.View;
using FubuMVC.Razor.RazorModel;

namespace FubuMVC.Razor
{
    public class RazorViewFacility : IViewFacility
    {
        public Task<IEnumerable<IViewToken>> FindViews(BehaviorGraph graph)
        {
            return Task.Factory.StartNew(() => findViews(graph));
        }

        private IEnumerable<IViewToken> findViews(BehaviorGraph graph)
        {
            var razorSettings = graph.Settings.Get<RazorEngineSettings>();
            var namespaces = graph.Settings.Get<CommonViewNamespaces>();

            var factory = new TemplateFactoryCache(namespaces, razorSettings, new TemplateCompiler(),
                new RazorTemplateGenerator());

            return graph.Files.FindFiles(razorSettings.Search)
                .Select(file => {
                    var template = new RazorTemplate(file);

                    return new RazorViewToken(template, factory);
                });
        } 

    }
}