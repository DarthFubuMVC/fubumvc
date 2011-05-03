using System.Collections.Generic;
using System.IO;
using FubuCore;

namespace FubuMVC.Spark.SparkModel
{
    public interface ISharedDirectoryProvider
    {
        IEnumerable<string> GetDirectories(SparkItem item, IEnumerable<SparkItem> items);
    }

    public class SharedDirectoryProvider : ISharedDirectoryProvider
    {
        private readonly ISharedPathBuilder _builder;

        public SharedDirectoryProvider() : this(new SharedPathBuilder())
        {
        }

        public SharedDirectoryProvider(ISharedPathBuilder builder)
        {
            _builder = builder;
        }

        public IEnumerable<string> GetDirectories(SparkItem item, IEnumerable<SparkItem> items)
        {
            foreach (var directory in _builder.BuildFrom(item.RootPath, item.FilePath))
            {
                yield return directory;
            }
			
            if (item.Origin == FubuSparkConstants.HostOrigin)
            {
                yield break;
            }
			
            var hostRoot = findHostRoot(items);
            if (hostRoot.IsEmpty())
            {
                yield break;
            }

            foreach (var sharedFolder in _builder.SharedFolderNames)
            {
                yield return Path.Combine(hostRoot, sharedFolder);
            }
        }
		
        private static string findHostRoot(IEnumerable<SparkItem> items)
        {
            return items.ByOrigin(FubuSparkConstants.HostOrigin).FirstValue(x => x.RootPath);
        }
    }
}