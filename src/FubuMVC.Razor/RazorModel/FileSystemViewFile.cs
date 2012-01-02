using System.IO;

namespace FubuMVC.Razor.FileSystem
{
    public class FileSystemViewFile : IViewFile
    {
        private readonly string _fullPath;
        private long _lastModified;
        private string _source;
        private string _extension;

        public long LastModified
        {
            get { return File.GetLastWriteTimeUtc(_fullPath).Ticks; }
        }

        public string Extension
        {
            get { return _extension; }
        }

        public FileSystemViewFile(string fullPath)
        {
            _fullPath = fullPath;
            _lastModified = LastModified;
            var fileInfo = new FileInfo(fullPath);
            _extension = fileInfo.Extension;
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
        string Extension { get; }
        long LastModified { get; }
        bool IsCurrent();
        Stream OpenViewStream();
        string GetSourceCode();
    }
}