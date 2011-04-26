using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;

namespace FubuMVC.Spark.SparkModel
{
    public interface ISharedItemLocator
    {
        SparkItem LocateSpark(string sparkName, SparkItem fromItem, SparkItems itemPool);
    }

    public class SharedItemLocator : ISharedItemLocator
    {
        private readonly IEnumerable<string> _sharedFolderNames;
        public SharedItemLocator(IEnumerable<string> sharedFolderNames)
        {
            _sharedFolderNames = sharedFolderNames;
        }

        public SparkItem LocateSpark(string sparkName, SparkItem fromItem, SparkItems itemPool)
        {
            var spark = locateSpark(sparkName, fromItem.FilePath, fromItem.RootPath, itemPool);
            if (spark == null && fromItem.Origin != Constants.HostOrigin)
            {
                spark = locateInHostFromPackage(sparkName, itemPool);
            }

            return spark;
        }

        private SparkItem locateSpark(string sparkName, string startPath, string stopPath, SparkItems itemPool)
        {
            var reachables = reachableLocations(startPath, stopPath).ToList();
            return itemPool.ByName(sparkName)
                .Where(x => reachables.Contains(x.DirectoryPath()))
                .FirstOrDefault();            
        }

        private SparkItem locateInHostFromPackage(string sparkName, SparkItems itemPool)
        {
            var rootItem = itemPool.FirstOrDefault(x => x.Origin == Constants.HostOrigin);
            if (rootItem == null) return null;

            var sharedFolder = _sharedFolderNames.FirstValue(p => p);
            var stopPath = rootItem.RootPath;
            var startPath = Path.Combine(stopPath, sharedFolder);

            return locateSpark(sparkName, startPath, stopPath, itemPool);
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