using System.IO;

namespace FubuMVC.Razor.FileSystem
{
    public class FileSystemViewFile : IViewFile
    {
        private readonly string _fullPath;
        private long _lastModified;
        private string _source;

        public long LastModified
        {
            get { return File.GetLastWriteTimeUtc(_fullPath).Ticks; }
        }

        public FileSystemViewFile(string fullPath)
        {
            _fullPath = fullPath;
            _lastModified = LastModified;
        }

        public Stream OpenViewStream()
        {
            return new FileStream(_fullPath, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Write);
        }

        public string GetSourceCode()
        {
            using(var fileStream = OpenViewStream())
            using(var reader = new StreamReader(fileStream))
            {
                _source = reader.ReadToEnd();
            }
            return _source;
        }

        public bool IsCurrent()
        {
            return _lastModified == LastModified;
        }
    }

    public interface IViewFile
    {
        long LastModified { get; }
        bool IsCurrent();
        Stream OpenViewStream();
        string GetSourceCode();
    }
}