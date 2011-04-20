namespace FubuMVC.Spark.SparkModel.Scanning
{
    public class FileFound
    {
        public FileFound(string path, string root, string directory)
        {
            Path = path;
            Root = root;
            Directory = directory;
        }

        public string Path { get; private set; }
        public string Root { get; private set; }
        public string Directory { get; private set; }
    }
}