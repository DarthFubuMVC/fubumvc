using System.IO;

namespace FubuMVC.Razor.FileSystem
{
    public class FileSystemViewFile : IViewFile
    {
        private readonly string _fullPath;

        public long LastModified
        {
            get { return File.GetLastWriteTimeUtc(_fullPath).Ticks; }
        }

        public FileSystemViewFile(string fullPath)
        {
            _fullPath = fullPath;
        }

        public Stream OpenViewStream()
        {
            return new FileStream(_fullPath, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Write);
        }
    }

    public interface IViewFile
    {
        long LastModified { get; }
        Stream OpenViewStream();
    }
}