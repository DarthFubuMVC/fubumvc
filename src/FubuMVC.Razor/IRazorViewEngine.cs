namespace FubuMVC.Razor
{
    public interface IRazorViewEngine
    {
        IRazorViewEntry CreateEntry(RazorViewDescriptor descriptor);
    }
}