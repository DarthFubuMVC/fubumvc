using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Spark.SparkModel
{
    public interface ISharedItemLocator
    {
        SparkItem LocateItem(string sparkName, SparkItem fromItem, IEnumerable<SparkItem> items);
    }
	
	// REVIEW: SharedItemLocator could as well just be merged with that DirectoryProvider as it
	// does not do anything.
	
    public class SharedItemLocator : ISharedItemLocator
    {
        private readonly ISharedDirectoryProvider _sharedDirectoryProvider;

        public SharedItemLocator() : this(new SharedDirectoryProvider())
        {
        }
        public SharedItemLocator(ISharedDirectoryProvider sharedDirectoryProvider)
        {
            _sharedDirectoryProvider = sharedDirectoryProvider;
        }

        public SparkItem LocateItem(string sparkName, SparkItem fromItem, IEnumerable<SparkItem> items)
        {
            var reachables = _sharedDirectoryProvider.GetDirectories(fromItem, items);
            return items.ByName(sparkName).InDirectories(reachables).FirstOrDefault();
        }
    }
}