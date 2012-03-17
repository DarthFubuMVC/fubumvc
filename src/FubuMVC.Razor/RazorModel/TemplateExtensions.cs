using System.IO;
using FubuCore;
using FubuMVC.Core.View.Model;

namespace FubuMVC.Razor.RazorModel
{
    public static class TemplateExtensions
    {
        public static bool IsRazorView(this ITemplateFile template)
        {
            return template.FilePath.FileExtension().EqualsIgnoreCase(".cshtml")
                   || template.FilePath.FileExtension().EqualsIgnoreCase(".vbhtml");
        }

        public static string FileExtension(this string fileName)
        {
            return Path.GetExtension(fileName);
        }

        public static long LastModified(this string fileName)
        {
            return File.GetLastWriteTimeUtc(fileName).Ticks;
        }
    }
}