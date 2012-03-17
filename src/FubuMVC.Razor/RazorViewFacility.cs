using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Model;
using FubuMVC.Razor.RazorModel;

namespace FubuMVC.Razor
{
    public class RazorViewFacility : IViewFacility
    {
        private readonly ITemplateRegistry<IRazorTemplate> _templateRegistry;

        public RazorViewFacility(ITemplateRegistry<IRazorTemplate> templateRegistry)
        {
            _templateRegistry = templateRegistry;
        }

        public IEnumerable<IViewToken> FindViews(TypePool types, BehaviorGraph graph)
        {
            // clean up pending
            return _templateRegistry
                .AllTemplates()
                .Where(x => x.Descriptor is ViewDescriptor<IRazorTemplate>)
                .Select(x => x.Descriptor.As<ViewDescriptor<IRazorTemplate>>())
                .Where(x => x.HasViewModel())
                .Select(x => new RazorViewToken(x));
        }
    }
}