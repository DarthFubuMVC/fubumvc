using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.View;
using FubuMVC.Razor.RazorModel;

namespace FubuMVC.Razor
{
    public class RazorViewFacility : IViewFacility
    {
        private readonly ITemplateRegistry _templateRegistry;

        public RazorViewFacility(ITemplateRegistry templateRegistry)
        {
            _templateRegistry = templateRegistry;
        }

        public IEnumerable<IViewToken> FindViews(TypePool types, BehaviorGraph graph)
        {
            // clean up pending
            return _templateRegistry
                .AllTemplates()
                .Where(x => x.Descriptor is ViewDescriptor)
                .Select(x => x.Descriptor.As<ViewDescriptor>())
                .Where(x => x.HasViewModel())
                .Select(x => new RazorViewToken(x));
        }
    }
}