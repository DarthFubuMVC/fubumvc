using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;

namespace FubuMVC.Spark.SparkModel
{
    public interface ISharedItemLocator
    {
        SparkItem LocateItem(string sparkName, SparkItem fromItem, IEnumerable<SparkItem> items);
    }
	
    public class SharedItemLocator : ISharedItemLocator
    {
        private readonly SharedDirectoryProvider _sharedDirectoryProvider;
        public SharedItemLocator(IEnumerable<string> sharedFolderNames)
        {
            _sharedDirectoryProvider = new SharedDirectoryProvider(sharedFolderNames);
        }

        public SparkItem LocateItem(string sparkName, SparkItem fromItem, IEnumerable<SparkItem> items)
        {
            var reachables = _sharedDirectoryProvider.GetDirectories(fromItem, items);
            var item = items.ByName(sparkName).InDirectories(reachables).FirstOrDefault();
            return item;
        }
    }

    public class SharedFolderFinder
    {
        private readonly IEnumerable<string> _sharedFolderNames;
        public SharedFolderFinder(IEnumerable<string> sharedFolderNames)
        {
            _sharedFolderNames = sharedFolderNames;
        }

        public IEnumerable<string> Find(string root, string path)
        {
            return reachableLocations(root, path);
        }
        private IEnumerable<string> reachableLocations(string root, string path)
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

    public class SharedDirectoryProvider
    {
        private readonly IEnumerable<string> _sharedFolderNames;
        private readonly SharedFolderFinder _sharedFolderFinder;

        public SharedDirectoryProvider(IEnumerable<string> sharedFolderNames)
        {
            _sharedFolderNames = sharedFolderNames;
            _sharedFolderFinder = new SharedFolderFinder(sharedFolderNames);
        }

        public IEnumerable<string> GetDirectories(SparkItem item, IEnumerable<SparkItem> items)
        {
            var candidates = getCandidateDirectories(item, items);
            return candidates;
        }
        private IEnumerable<string> getCandidateDirectories(SparkItem item, IEnumerable<SparkItem> items)
        {
            foreach (var directory in _sharedFolderFinder.Find(item.RootPath, item.FilePath))
            {
                yield return directory;
            }
            if (item.Origin == FubuSparkConstants.HostOrigin)
            {
                yield break;
            }
            var hostRoot = items.ByOrigin(FubuSparkConstants.HostOrigin).FirstValue(x => x.RootPath);
            if (hostRoot.IsEmpty())
            {
                yield break;
            }
            foreach (var sharedFolder in _sharedFolderNames)
            {
                yield return Path.Combine(hostRoot, sharedFolder);
            }
        }
    }
}