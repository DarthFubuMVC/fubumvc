using FubuMVC.Razor.RazorModel;

namespace FubuMVC.Razor.RazorEngine
{
    public interface IRazorViewEntryFactory
    {
        IRazorViewEntry CreateEntry(ViewDescriptor descriptor);
    }
}