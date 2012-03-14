using System.IO;
using FubuCore;
using FubuMVC.Core.View.Model;

namespace FubuMVC.Razor.RazorModel
{
    public static class TemplateExtensions
    {
        public static bool IsRazorView(this ITemplateFile template)
        {
            return Path.GetExtension(template.FilePath).EqualsIgnoreCase(".cshtml")
                   || Path.GetExtension(template.FilePath).EqualsIgnoreCase(".vbhtml");
        }
    }
}