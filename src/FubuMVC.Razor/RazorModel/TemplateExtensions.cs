using System.IO;
using FubuCore;

namespace FubuMVC.Razor.RazorModel
{
    public static class TemplateExtensions
    {
        public static string RelativePath(this IRazorTemplate template)
        {
            return template.FilePath.PathRelativeTo(template.RootPath);
        }

        public static string DirectoryPath(this IRazorTemplate template)
        {
            return Path.GetDirectoryName(template.FilePath);
        }

        public static string RelativeDirectoryPath(this IRazorTemplate template)
        {
            return template.DirectoryPath().PathRelativeTo(template.RootPath);
        }

        public static string Name(this IRazorTemplate template)
        {
            return Path.GetFileNameWithoutExtension(template.FilePath);
        }

        public static bool IsRazorView(this IRazorTemplate template)
		{
            return Path.GetExtension(template.FilePath).EqualsIgnoreCase(".cshtml");
        }

        public static bool IsXml(this IRazorTemplate template)
		{
            return Path.GetExtension(template.FilePath).EqualsIgnoreCase(".xml");
        }

        public static bool FromHost(this IRazorTemplate template)
        {
            return template.Origin == FubuRazorConstants.HostOrigin;
        }
    }
}