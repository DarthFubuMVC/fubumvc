using System.Collections.ObjectModel;
using System.IO;
using FubuCore;

namespace FubuMVC.Spark.Scanning
{
    public class SparkFile
    {
        public SparkFile(string path, string root, string origin)
        {
            Path = path;
            Root = root;
            Origin = origin;
        }

        public string Path { get; private set; }
        public string Root { get; private set; }
        public string Origin { get; private set; }
    }

    public class SparkFiles : Collection<SparkFile>
    {
    }

    public static class SparkFileHelper
    {
        public static string Namespace(this SparkFile file)
        {
            return Path.GetDirectoryName(file.Path).PathRelativeTo(file.Root).Replace(Path.DirectorySeparatorChar, '.');
        }

        public static string RelativePath(this SparkFile file)
        {
            return file.Path.PathRelativeTo(file.Root);
        }

        public static string Name(this SparkFile file)
        {
            return Path.GetFileNameWithoutExtension(file.Path);
        }

        public static string Extension(this SparkFile file)
        {
            return Path.GetExtension(file.Path);
        }
    }
}