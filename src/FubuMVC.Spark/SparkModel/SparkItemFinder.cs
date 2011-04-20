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

        // Later : Take the variable part of this into a search object
        private ScanRequest buildRequest(ICollection<SparkItem> files)
        {
            //// TODO: Extract and allow for configuration / convention
            var request = new ScanRequest();
            request.Include("*.spark");
            request.Include("bindings.xml");
            ////

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