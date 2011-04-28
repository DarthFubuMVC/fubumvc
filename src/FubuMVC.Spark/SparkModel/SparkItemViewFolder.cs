using System.Collections.Generic;
using System.IO;
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
        private readonly Cache<string, string> _getFilePath;

        public SparkItemViewFolder(IEnumerable<SparkItem> items)
        {
            _items = items;
            _listViews = new Cache<string, IList<string>>(listViews);
            _hasView = new Cache<string, bool>(hasView);
            _getFilePath = new Cache<string, string>(getFilePath);
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
            return new FileSystemViewFile(_getFilePath[path]);
        }

        private IList<string> listViews(string path)
        {
            return _items
               .Where(x => Path.GetDirectoryName(x.ViewPath) == path)
               .Select(x => x.ViewPath)
               .ToList();
        }

        private bool hasView(string path)
        {
            return _items.Any(x => x.ViewPath == path);
        }

        private string getFilePath(string path)
        {
            return _items.Where(x => x.ViewPath == path).First().FilePath;
        }
    }
}