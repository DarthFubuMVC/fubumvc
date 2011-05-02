using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using FubuCore.Util;

namespace FubuMVC.Spark.SparkModel
{
    public static class SparkItemExtensions
    {
        public static string RelativePath(this SparkItem item)
        {
            return item.FilePath.PathRelativeTo(item.RootPath);
        }

        public static string DirectoryPath(this SparkItem item)
        {
            return Path.GetDirectoryName(item.FilePath);
        }

        public static string RelativeDirectoryPath(this SparkItem item)
        {
            return item.DirectoryPath().PathRelativeTo(item.RootPath);
        }

        public static string Name(this SparkItem item)
        {
            return Path.GetFileNameWithoutExtension(item.FilePath);
        }

        public static bool HasViewModel(this SparkItem item)
        {
            return item.ViewModelType != null;
        }
    }

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

        public static IEnumerable<SparkItem> InDirectories(this IEnumerable<SparkItem> items, IEnumerable<string> directories)
        {
            var predicate = new CompositePredicate<SparkItem>();
            predicate = directories.Aggregate(predicate, (current, local) => current + (x => x.DirectoryPath() == local));
            return items.Where(predicate.MatchesAny);
        }
        public static SparkItem FirstByName(this IEnumerable<SparkItem> items, string name)
        {
            return items.ByName(name).FirstOrDefault();
        }
    }
}