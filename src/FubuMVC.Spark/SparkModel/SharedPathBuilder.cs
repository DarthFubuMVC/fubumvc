using System.Collections.Generic;
using System.IO;
using FubuCore;
using Spark;

namespace FubuMVC.Spark.SparkModel
{
	public interface ISharedPathBuilder
    {
        IEnumerable<string> BuildFrom(string path, string root);
        IEnumerable<string> SharedFolderNames { get; }
    }
	
    public class SharedPathBuilder : ISharedPathBuilder
    {
        private readonly IEnumerable<string> _sharedFolderNames;

        public SharedPathBuilder() : this(new[] { Constants.Shared }) {}
        public SharedPathBuilder(IEnumerable<string> sharedFolderNames)
        {
            _sharedFolderNames = sharedFolderNames;
        }

        public IEnumerable<string> BuildFrom(string path, string root)
        {
            if (path == root) yield break;

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

        public IEnumerable<string> SharedFolderNames
        {
            get { return _sharedFolderNames; }
        }
    }
}
