using System.IO;
using FubuCore;

namespace FubuMVC.Razor.RazorModel
{
    public static class TemplateExtensions
    {
        public static bool IsRazorView(this IRazorTemplate template)
        {
            return Path.GetExtension(template.FilePath).EqualsIgnoreCase(".cshtml")
                   || Path.GetExtension(template.FilePath).EqualsIgnoreCase(".vbhtml");
        }
    }
}