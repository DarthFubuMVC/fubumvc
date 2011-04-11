using System.Collections.ObjectModel;

namespace FubuMVC.Spark.Scanning
{
    public class SparkFile
    {
        public SparkFile(string path, string root)
        {
            Path = path;
            Root = root;
        }

        public string Path { get; private set; }
        public string Root { get; private set; }
    }

    public class SparkFiles : Collection<SparkFile> { } 
}