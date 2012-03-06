using System.IO;
using FubuCore;

namespace FubuMVC.Core.View.Model
{
    public static class TemplateExtensions
    {
        public static string RelativePath(this ITemplateFile template)
        {
            return template.FilePath.PathRelativeTo(template.RootPath);
        }

        public static string DirectoryPath(this ITemplateFile template)
        {
            return Path.GetDirectoryName(template.FilePath);
        }

        public static string RelativeDirectoryPath(this ITemplateFile template)
        {
            return template.DirectoryPath().PathRelativeTo(template.RootPath);
        }

        public static string Name(this ITemplateFile template)
        {
            return Path.GetFileNameWithoutExtension(template.FilePath);
        }

        public static bool FromHost(this ITemplateFile template)
        {
            return template.Origin == TemplateConstants.HostOrigin;
        }
    }

    public static class TemplateConstants
    {
        public const string HostOrigin = "Host";
    }
}