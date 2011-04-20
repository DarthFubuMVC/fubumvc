using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;
using Spark.FileSystem;

namespace FubuMVC.Spark.SparkModel
{
    public class SparkItemViewFolder : IViewFolder
    {
        private readonly IEnumerable<SparkItem> _items;
        private readonly Cache<string, IList<string>> _listViews;
        private readonly Cache<string, bool> _hasView;
        private readonly Cache<string, string> _getViewPath;

        public SparkItemViewFolder(IEnumerable<SparkItem> items)
        {
            _items = items;
            _listViews = new Cache<string, IList<string>>(listViews);
            _hasView = new Cache<string, bool>(hasView);
            _getViewPath = new Cache<string, string>(getViewPath);
        }

        public IList<string> ListViews(string path)
        {
            return _listViews[path];
        }

        public bool HasView(string path)
        {
            return _hasView[path];
        }

        public IViewFile GetViewSource(string path)
        {
            var filePath = _getViewPath[path];
            return new FileSystemViewFile(filePath);
        }

        private IList<string> listViews(string path)
        {
            return _items
               .Where(x => x.PrefixedRelativeDirectoryPath == path)
               .Select(x => x.PrefixedRelativePath)
               .ToList();
        }

        private bool hasView(string path)
        {
            return _items.Any(x => x.PrefixedRelativePath == path);
        }

        private string getViewPath(string path)
        {
            var item = _items.Where(x => x.PrefixedRelativePath == path).First();
            return item.FilePath;
        }
    }
}