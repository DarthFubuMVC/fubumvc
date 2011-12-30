namespace FubuMVC.Razor.RazorEngine
{
    public interface IRazorViewEngine
    {
        IRazorViewEntry CreateEntry(RazorViewDescriptor descriptor);
    }
}