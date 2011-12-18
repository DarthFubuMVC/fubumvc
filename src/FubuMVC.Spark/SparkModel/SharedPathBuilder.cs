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
	    IEnumerable<string> BuildBy(string directoryPath);
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
            return buildBy(path, root, includeDirectAncestor).ToList().Distinct();
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
                foreach (var sharedPath in BuildBy(path))
                {
                    yield return sharedPath;
                }

            } while (path.IsNotEmpty() && path.PathRelativeTo(root).IsNotEmpty());            
        } 

        public IEnumerable<string> BuildBy(string directoryPath)
        {
            return _sharedFolderNames.Select(s => Path.Combine(directoryPath, s)).Distinct();
        }
    }
}
