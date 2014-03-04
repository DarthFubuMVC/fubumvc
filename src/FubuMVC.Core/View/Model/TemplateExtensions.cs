using System.IO;
using FubuCore;
using FubuMVC.Core.Runtime.Files;

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

        public static bool IsPartial(this ITemplateFile template)
        {
            return Path.GetFileName(template.FilePath).StartsWith("_");
        }
    }

    public static class TemplateConstants
    {
        public const string Shared = "Shared";
        public static readonly string HostOrigin = ContentFolder.Application;
    }
}