
namespace FubuMVC.Razor.Rendering
{
    public class ViewDefinition
    {
        public ViewDefinition(RazorViewDescriptor viewDescriptor, RazorViewDescriptor partialDescriptor)
        {
            ViewDescriptor = viewDescriptor;
            PartialDescriptor = partialDescriptor;
        }
        public RazorViewDescriptor ViewDescriptor { get; private set; }
        public RazorViewDescriptor PartialDescriptor { get; private set; }
    }
}