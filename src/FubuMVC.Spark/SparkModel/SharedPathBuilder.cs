using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using Spark;

namespace FubuMVC.Spark.SparkModel
{
	public interface ISharedPathBuilder
    {
        IEnumerable<string> BuildBy(string path, string root, bool includeDirectAncestor);
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

        public IEnumerable<string> BuildBy(string path, string root, bool includeDirectAncestor)
        {
            return buildBy(path, root, includeDirectAncestor)
                .ToList()
                .Distinct();
        }

        private IEnumerable<string> buildBy(string path, string root, bool includeDirectAncestor)
        {
            if (path == root) yield break;

            do
            {
                path = Path.GetDirectoryName(path);
                if (path == null) break;

                if (includeDirectAncestor)
                {
                    yield return path;
                }
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
