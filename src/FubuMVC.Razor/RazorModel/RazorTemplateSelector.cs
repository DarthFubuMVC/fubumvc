using FubuMVC.Core.View.Model;

namespace FubuMVC.Razor.RazorModel
{
    public class RazorTemplateSelector : ITemplateSelector<IRazorTemplate>
    {
        public bool IsAppropriate(IRazorTemplate template)
        {
            return template.IsRazorView();
        }
    }
}