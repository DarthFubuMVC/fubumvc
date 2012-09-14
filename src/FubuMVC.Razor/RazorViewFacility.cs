using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Model;
using FubuMVC.Razor.RazorModel;

namespace FubuMVC.Razor
{
    public class RazorViewFacility : IViewFacility
    {
        private readonly TemplateRegistry<IRazorTemplate> _templateRegistry;
        private readonly RazorParsings _razorParsings;

        public RazorViewFacility(TemplateRegistry<IRazorTemplate> templateRegistry, RazorParsings razorParsings)
        {
            _templateRegistry = templateRegistry;
            _razorParsings = razorParsings;
        }

        public IEnumerable<IViewToken> FindViews(BehaviorGraph graph)
        {
            var razorSettings = graph.Settings.Get<RazorEngineSettings>();
            RegisterTemplates(graph.Files, razorSettings);
            ComposeTemplates(razorSettings);

            return FindTokens();
        }
        
        public IEnumerable<IViewToken> FindTokens()
        {            
            return _templateRegistry.DescriptorsWithViewModels<ViewDescriptor<IRazorTemplate>>()
                .Select(x => new RazorViewToken(x));
        } 

        public void RegisterTemplates(IFubuApplicationFiles fubuFiles, RazorEngineSettings settings)
        {
            fubuFiles.FindFiles(settings.Search).Each(file => 
                _templateRegistry.Register(new Template(file.Path, file.ProvenancePath, file.Provenance)));            
        }

        public void ComposeTemplates(RazorEngineSettings settings)
        {
            _templateRegistry.Each(_razorParsings.Parse);

            var composer = new TemplateComposer<IRazorTemplate>(_razorParsings);
            settings.Configure(composer);            
            composer.Compose(_templateRegistry);
        }
    }
}