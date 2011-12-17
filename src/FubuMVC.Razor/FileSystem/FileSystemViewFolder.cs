using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FubuMVC.Razor.FileSystem
{
    public class FileSystemViewFolder : IViewFolder
    {
        private readonly string _basePath;

        public FileSystemViewFolder(string basePath)
        {
            _basePath = basePath;
        }

        public string BasePath
        {
            get { return _basePath; }
        }


        public IViewFile GetViewSource(string path)
        {
            string fullPath = Path.Combine(_basePath, path);
            if (!File.Exists(fullPath))
                throw new FileNotFoundException("View source file not found.", fullPath);

            return new FileSystemViewFile(fullPath);
        }

        public IList<string> ListViews(string path)
        {
            if (!Directory.Exists(Path.Combine(_basePath, path)))
                return new string[0];

            var files = Directory.GetFiles(Path.Combine(_basePath, path));
            return files.ToList().ConvertAll(viewPath => Path.GetFileName(viewPath));
        }

        public bool HasView(string path)
        {
            return File.Exists(Path.Combine(_basePath, path));
        }
    }

    public interface IViewFolder
    {
        IViewFile GetViewSource(string path);

        IList<string> ListViews(string path);

        bool HasView(string path);
    }
}