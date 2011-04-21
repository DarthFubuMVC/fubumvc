using System.Collections.Generic;
using System.Linq;
using FubuMVC.Spark.SparkModel.Scanning;

namespace FubuMVC.Spark.SparkModel
{
    public interface ISparkItemFinder
    {
        IEnumerable<SparkItem> FindItems();
    }

    public class SparkItemFinder : ISparkItemFinder
    {
        private readonly IFileScanner _fileScanner;
        private readonly IEnumerable<SparkRoot> _sparkRoots;
        private readonly ScanRequest _request;

        public SparkItemFinder() : this(new FileScanner(), new SparkRoots()) {}
        public SparkItemFinder(IFileScanner fileScanner, IEnumerable<SparkRoot> sparkRoots)
        {
            _fileScanner = fileScanner;
            _sparkRoots = sparkRoots;
            _request = new ScanRequest();
            _request.Include("*.spark");
            _request.Include("bindings.xml");
        }

        public IEnumerable<SparkItem> FindItems()
        {
            var items = new List<SparkItem>();
            var request = buildRequest(items);
            _fileScanner.Scan(request);
            return items;
        }

        public void Include(string filter)
        {
            _request.Include(filter);
        }


        // Later : Take the variable part of this into a search object
        private ScanRequest buildRequest(ICollection<SparkItem> files)
        {
            _sparkRoots.Each(r => _request.AddRoot(r.Path));

            _request.AddHandler(fileFound =>
            {
                var origin = _sparkRoots.First(x => x.Path == fileFound.Root).Origin;
                var sparkFile = new SparkItem(fileFound.Path, fileFound.Root, origin);
                
                files.Add(sparkFile);
            });

            return _request;
        }
    }      
}