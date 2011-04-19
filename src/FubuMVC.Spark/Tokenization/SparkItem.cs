using System;
using System.Collections.ObjectModel;
using System.IO;
using FubuCore;

namespace FubuMVC.Spark.Tokenization
{
    public static class Constants
    {
        public const string HostOrigin = "Host";
    }

    public class SparkItem
    {
        public SparkItem(string path, string root, string origin)
        {
            Path = path;
            Root = root;
            Origin = origin;
        }

        public string Path { get; private set; }
        public string Root { get; private set; }
        public string Origin { get; private set; }

        public SparkItem Master { get; set; }
        public Type ViewModelType { get; set; }
        public string Namespace { get; set; }


        public override string ToString()
        {
            return Path;
        }

    }

    public static class SparkItemHelper
    {
        // NOTE:TEMP
        public static string PrefixedRelativePath(this SparkItem item)
        {
            return FileSystem.Combine(item.Origin, item.RelativePath());
        }

        // NOTE:TEMP
        public static string PrefixedVirtualDirectoryPath(this SparkItem item)
        {
            return FileSystem.Combine(item.Origin, item.VirtualDirectoryPath());
        }

        // NOTE:TEMP
        public static string VirtualDirectoryPath(this SparkItem item)
        {
            return item.DirectoryPath().PathRelativeTo(item.Root);
        }

        public static string RelativePath(this SparkItem item)
        {
            return item.Path.PathRelativeTo(item.Root);
        }

        public static string Name(this SparkItem item)
        {
            return Path.GetFileNameWithoutExtension(item.Path);
        }

        public static string DirectoryPath(this SparkItem item)
        {
            return Path.GetDirectoryName(item.Path);
        }
        public static bool HasViewModel(this SparkItem item)
        {
            return item.ViewModelType != null;
        }
    }

    public class SparkItems : Collection<SparkItem> { }
}