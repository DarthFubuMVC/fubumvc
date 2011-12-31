using RazorEngine.Templating;

namespace FubuMVC.Razor
{
    public interface ITemplateServiceWrapper
    {
        ITemplateService TemplateService { get; }
    }
}