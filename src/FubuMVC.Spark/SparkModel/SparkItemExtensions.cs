using System.IO;
using FubuCore;

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
}