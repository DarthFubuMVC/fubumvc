using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Model;
using FubuMVC.Razor.RazorModel;

namespace FubuMVC.Razor
{
    public class RazorViewFacility : IViewFacility
    {
        private readonly RazorParsings _razorParsings;

        public RazorViewFacility(RazorParsings razorParsings)
        {
            _razorParsings = razorParsings;
        }


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

            var composer = new TemplateComposer<IRazorTemplate>(_razorParsings);
            razorSettings.Configure(composer);

            var templates = graph.Files.FindFiles(razorSettings.Search)
                .Select(file => {
                    var template = new RazorTemplate(file);

                    _razorParsings.Parse(template);
                    composer.Compose(template);

                    return template;
                });


            return templates.Select(x => new RazorViewToken(x.Descriptor.As<ViewDescriptor<IRazorTemplate>>(), factory));
        } 

    }
}