using System.Collections.Generic;
using System.Linq;
using FubuMVC.Spark.Tokenization;
using Spark.FileSystem;

namespace FubuMVC.Spark
{
    public class SparkItemViewFolder : IViewFolder
    {
        private readonly IEnumerable<SparkItem> _items;
        public SparkItemViewFolder(IEnumerable<SparkItem> items)
        {
            _items = items;
        }

        public IList<string> ListViews(string path)
        {
            return _items
                .Where(x => x.RelativeDirectoryPath() == path)
                .Select(x => x.RelativePath())
                .ToList();
        }

        public bool HasView(string path)
        {
            return _items.Any(x => x.RelativePath() == path);
        }

        public IViewFile GetViewSource(string path)
        {
            var item = _items.Where(x => x.RelativePath() == path).First();
            return new FileSystemViewFile(item.FilePath);
        }
    }
}