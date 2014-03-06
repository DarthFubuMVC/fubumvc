using FubuMVC.Core.View.Model;

namespace FubuMVC.Razor.RazorModel
{
    public class RazorTemplateSelector : ITemplateSelector<RazorTemplate>
    {
        public bool IsAppropriate(RazorTemplate template)
        {
            return template.IsRazorView();
        }
    }
}