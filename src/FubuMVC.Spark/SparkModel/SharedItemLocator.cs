using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;

namespace FubuMVC.Spark.SparkModel
{
    public interface ISharedItemLocator
    {
        SparkItem LocateSpark(string sparkName, SparkItem fromItem);
    }

    // TODO: Find a better way to split pack vs host concerns up.
    public class SharedItemLocator : ISharedItemLocator
    {
        private readonly SparkItems _items;
        private readonly IEnumerable<string> _sharedFolderNames;

        public SharedItemLocator(SparkItems items, IEnumerable<string> sharedFolderNames)
        {
            _items = items;
            _sharedFolderNames = sharedFolderNames;
        }

        public SparkItem LocateSpark(string sparkName, SparkItem fromItem)
        {
            var spark = locateSpark(sparkName, fromItem.FilePath, fromItem.RootPath);
            if (spark == null && fromItem.Origin != Constants.HostOrigin)
            {
                spark = locateInHostFromPackage(sparkName);
            }

            return spark;
        }

        private SparkItem locateSpark(string sparkName, string startPath, string stopPath)
        {
            var reachables = reachableLocations(startPath, stopPath).ToList();
            return _items.ByName(sparkName)
                .Where(x => reachables.Contains(x.DirectoryPath()))
                .FirstOrDefault();            
        }

        private SparkItem locateInHostFromPackage(string sparkName)
        {
            var rootItem = _items.FirstOrDefault(x => x.Origin == Constants.HostOrigin);
            if (rootItem == null) return null;

            var sharedFolder = _sharedFolderNames.FirstValue(p => p);
            var stopPath = rootItem.RootPath;
            var startPath = Path.Combine(stopPath, sharedFolder);

            return locateSpark(sparkName, startPath, stopPath);
        }

        private IEnumerable<string> reachableLocations(string path, string root)
        {
            do
            {
                path = Path.GetDirectoryName(path);
                if (path == null) break;

                foreach (var sharedFolder in _sharedFolderNames)
                {
                    yield return Path.Combine(path, sharedFolder);
                }

            } while (path.IsNotEmpty() && path.PathRelativeTo(root).IsNotEmpty());
        }
    }
}