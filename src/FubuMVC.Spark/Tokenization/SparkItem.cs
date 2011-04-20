using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public string RelativeDirectoryPath()
        {
            return DirectoryPath().PathRelativeTo(RootPath);
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
            var prefix = item.Origin == Constants.HostOrigin ? string.Empty : item.Origin;
            return FileSystem.Combine(prefix, item.RelativePath());
        }

        // NOTE:TEMP
        public static string PrefixedRelativeDirectoryPath(this SparkItem item)
        {
            var prefix = item.Origin == Constants.HostOrigin ? string.Empty : item.Origin;
            return FileSystem.Combine(prefix, item.RelativeDirectoryPath());
        }
    }

    // We need to get this populated once and then pass around an interface.
    public class SparkItems : List<SparkItem> // TODO: Create lookup registry interface from this
    {
        public SparkItems(){}
        public SparkItems(IEnumerable<SparkItem> items) : base(items) {}

        // Temporary : probably ends up as extension method on IEnumerable...
        public SparkItem FirstByName(string name)
        {
            return this.FirstOrDefault(x => x.Name() == name);
        }
    }
}