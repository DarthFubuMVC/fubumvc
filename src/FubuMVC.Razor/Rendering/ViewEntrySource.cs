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
        public ViewEntrySource(ViewDescriptor descriptor, IViewEntryProviderCache provider)
        {
            _descriptor = descriptor;
            _provider = provider;
        }

        public IRazorViewEntry GetViewEntry()
        {
            return _provider.GetViewEntry(_descriptor);
        }

        public IRazorViewEntry GetPartialViewEntry()
        {
            return _provider.GetViewEntry(_descriptor);
        }
    }
}