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
            var currentModified = LastModified;
            if (_source != null && _lastModified == currentModified)
                return _source;
            using(var fileStream = OpenViewStream())
            using(var reader = new StreamReader(fileStream))
            {
                _source = reader.ReadToEnd();
            }
            _lastModified = currentModified;
            return _source;
        }
    }

    public interface IViewFile
    {
        long LastModified { get; }
        Stream OpenViewStream();
        string GetSourceCode();
    }
}