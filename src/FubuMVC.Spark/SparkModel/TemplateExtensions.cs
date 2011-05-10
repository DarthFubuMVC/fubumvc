using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using Spark;

namespace FubuMVC.Spark.SparkModel
{
    public static class TemplateExtensions
    {
        public static string RelativePath(this ITemplate template)
        {
            return template.FilePath.PathRelativeTo(template.RootPath);
        }

        public static string DirectoryPath(this ITemplate template)
        {
            return Path.GetDirectoryName(template.FilePath);
        }

        public static string RelativeDirectoryPath(this ITemplate template)
        {
            return template.DirectoryPath().PathRelativeTo(template.RootPath);
        }

        public static string Name(this ITemplate template)
        {
            return Path.GetFileNameWithoutExtension(template.FilePath);
        }

        public static bool IsPartial(this ITemplate template)
		{
            return Path.GetFileName(template.FilePath).StartsWith("_") && template.IsSparkView();
        }

        public static bool IsSparkView(this ITemplate template)
		{
            return Path.GetExtension(template.FilePath).Equals(Constants.DotSpark);
        }

        public static bool IsXml(this ITemplate template)
		{
            return Path.GetExtension(template.FilePath).Equals(".xml");
        }
        public static bool FromHost(this ITemplate template)
        {
            return template.Origin == FubuSparkConstants.HostOrigin;
        }
    }

    // TODO: Reconsider this (ITemplate).

    public static class SparkItemEnumerableExtensions
    {
        public static IEnumerable<ITemplate> ByName(this IEnumerable<ITemplate> templates, string name)
        {
            return templates.Where(x => x.Name() == name);
        }

        public static IEnumerable<ITemplate> ByOrigin(this IEnumerable<ITemplate> templates, string origin)
        {
            return templates.Where(x => x.Origin == origin);
        }
        public static ITemplate FirstByName(this IEnumerable<ITemplate> templates, string name)
        {
            return templates.ByName(name).FirstOrDefault();
        }
		
		// TODO: UT
        public static IEnumerable<ITemplate> InDirectories(this IEnumerable<ITemplate> templates, IEnumerable<string> directories)
        {
            return directories.SelectMany(local => templates.Where(x => x.DirectoryPath() == local));
        }
    }
}