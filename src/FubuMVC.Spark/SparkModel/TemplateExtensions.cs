using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using FubuCore.Util;
using Spark;

namespace FubuMVC.Spark.SparkModel
{
    public static class TemplateExtensions
    {
        public static string RelativePath(this ITemplate item)
        {
            return item.FilePath.PathRelativeTo(item.RootPath);
        }

        public static string DirectoryPath(this ITemplate item)
        {
            return Path.GetDirectoryName(item.FilePath);
        }

        public static string RelativeDirectoryPath(this ITemplate item)
        {
            return item.DirectoryPath().PathRelativeTo(item.RootPath);
        }

        public static string Name(this ITemplate item)
        {
            return Path.GetFileNameWithoutExtension(item.FilePath);
        }

        public static bool IsPartial(this ITemplate item)
		{
            return Path.GetFileName(item.FilePath).StartsWith("_") && item.IsSparkView();
        }

        public static bool IsSparkView(this ITemplate item)
		{
            return Path.GetExtension(item.FilePath).Equals(Constants.DotSpark);
        }

        public static bool IsXml(this ITemplate item)
		{
            return Path.GetExtension(item.FilePath).Equals(".xml");
        }
    }

    // TODO: Reconsider this (ITemplate).

    public static class SparkItemEnumerableExtensions
    {
        public static IEnumerable<SparkItem> ByName(this IEnumerable<SparkItem> items, string name)
        {
            return items.Where(x => x.Name() == name);
        }

        public static IEnumerable<SparkItem> ByOrigin(this IEnumerable<SparkItem> items, string origin)
        {
            return items.Where(x => x.Origin == origin);
        }
        public static SparkItem FirstByName(this IEnumerable<SparkItem> items, string name)
        {
            return items.ByName(name).FirstOrDefault();
        }
		
		// TODO: UT
        public static IEnumerable<SparkItem> InDirectories(this IEnumerable<SparkItem> items, IEnumerable<string> directories)
        {
            var predicate = new CompositePredicate<SparkItem>();
            predicate = directories.Aggregate(predicate, (current, local) => current + (x => x.DirectoryPath() == local));
            return items.Where(predicate.MatchesAny);
        }

    }
}