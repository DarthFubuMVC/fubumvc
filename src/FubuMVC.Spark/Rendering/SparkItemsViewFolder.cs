using System.Collections.Generic;
using System.Linq;
using FubuMVC.Spark.Tokenization;
using Spark.FileSystem;

namespace FubuMVC.Spark.Rendering
{
    public class SparkItemsViewFolder : IViewFolder
    {
        private readonly IEnumerable<SparkItem> _items;

        public SparkItemsViewFolder(IEnumerable<SparkItem> items)
        {
            _items = items;
        }

        public IList<string> ListViews(string path)
        {
            return _items
                .Where(x => x.PrefixedVirtualDirectoryPath() == path)
                .Select(x => x.PrefixedRelativePath())
                .ToList();
        }

        public bool HasView(string path)
        {
            return _items.Any(x => x.PrefixedRelativePath() == path);
        }

        public IViewFile GetViewSource(string path)
        {
            var item = _items.Where(x => x.PrefixedRelativePath() == path).First();
            return new FileSystemViewFile(item.Path);
        }
    }
}