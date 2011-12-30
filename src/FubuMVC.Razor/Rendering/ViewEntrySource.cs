using System;
using FubuMVC.Razor.RazorEngine;
using FubuMVC.Razor.RazorModel;

namespace FubuMVC.Razor.Rendering
{
    public interface IViewEntrySource
    {
        IRazorViewEntry GetViewEntry();
        IRazorViewEntry GetPartialViewEntry();
    }

    public class ViewEntrySource : IViewEntrySource
    {
        private readonly IViewEntryProviderCache _provider;
        private readonly ViewDescriptor _descriptor;
        private readonly IViewDefinitionResolver _resolver;
        public ViewEntrySource(ViewDescriptor descriptor, IViewEntryProviderCache provider, IViewDefinitionResolver resolver)
        {
            _descriptor = descriptor;
            _provider = provider;
            _resolver = resolver;
        }

        public IRazorViewEntry GetViewEntry()
        {
            return getViewEntry(x => x.ViewDescriptor);
        }

        public IRazorViewEntry GetPartialViewEntry()
        {
            return getViewEntry(x => x.PartialDescriptor);
        }

        private IRazorViewEntry getViewEntry(Func<ViewDefinition, RazorViewDescriptor> sparkDescriptorSelector)
        {
            var definition = _resolver.Resolve(_descriptor);
            var razorDescriptor = sparkDescriptorSelector(definition);
            return _provider.GetViewEntry(razorDescriptor);
        }
    }

}