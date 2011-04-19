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
        public SparkItem(string filePath, string rootPath, string origin)
        {
            FilePath = filePath;
            RootPath = rootPath;
            Origin = origin;
        }

        public string FilePath { get; private set; }
        public string RootPath { get; private set; }
        public string Origin { get; private set; }

        public SparkItem Master { get; set; }
        public Type ViewModelType { get; set; }
        public string Namespace { get; set; }

        public string RelativePath()
        {
            return FilePath.PathRelativeTo(RootPath);
        }

        public string DirectoryPath()
        {
            return Path.GetDirectoryName(FilePath);
        }

        public string Name()
        {
            return Path.GetFileNameWithoutExtension(FilePath);
        }

        public bool HasViewModel()
        {
            return ViewModelType != null;
        }

        public override string ToString()
        {
            return FilePath;
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
            return item.DirectoryPath().PathRelativeTo(item.RootPath);
        }
    }

    public class SparkItems : Collection<SparkItem> { }
}