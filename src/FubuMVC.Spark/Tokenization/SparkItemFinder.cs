using System.Collections.Generic;
using System.Linq;
using FubuMVC.Spark.Tokenization.Scanning;

namespace FubuMVC.Spark.Tokenization
{
    public interface ISparkItemFinder
    {
        IEnumerable<SparkItem> FindItems();
    }

    public class SparkItemFinder : ISparkItemFinder
    {
        private readonly IFileScanner _fileScanner;
        private readonly IEnumerable<SparkRoot> _sparkRoots;

        public SparkItemFinder() : this(new FileScanner(), new SparkRoots()) {}
        public SparkItemFinder(IFileScanner fileScanner, IEnumerable<SparkRoot> sparkRoots)
        {
            _fileScanner = fileScanner;
            _sparkRoots = sparkRoots;
        }

        public IEnumerable<SparkItem> FindItems()
        {
            var items = new List<SparkItem>();
            var request = buildRequest(items);
            _fileScanner.Scan(request);
            return items;
        }

        private ScanRequest buildRequest(ICollection<SparkItem> files)
        {
            var request = new ScanRequest();
            request.Include("*.spark");
            // TODO : Allow for convention on this.
            request.Include("bindings.xml");

            _sparkRoots.Each(r => request.AddRoot(r.Path));

            request.AddHandler(fileFound =>
            {
                var origin = _sparkRoots.First(x => x.Path == fileFound.Root).Origin;
                var sparkFile = new SparkItem(fileFound.Path, fileFound.Root, origin);
                
                files.Add(sparkFile);
            });

            return request;
        }
    }      
}