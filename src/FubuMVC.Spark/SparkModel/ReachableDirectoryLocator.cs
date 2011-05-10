using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using Spark;

namespace FubuMVC.Spark.SparkModel
{
    public interface IReachableDirectoryLocator
    {
        IEnumerable<TemplateDirectory> GetDirectories(ITemplate template, IEnumerable<ITemplate> templates);
    }

    public class TemplateDirectory
    {
        public string Path { get; set; }
        public bool IsShared { get; set; }
    }

    public class ReachableDirectoryLocator : IReachableDirectoryLocator
    {
        private readonly IEnumerable<string> _sharedFolderNames;

        public ReachableDirectoryLocator()
            : this(new[] { Constants.Shared })
        {
        }

        public ReachableDirectoryLocator(IEnumerable<string> sharedFolderNames)
        {
            _sharedFolderNames = sharedFolderNames;
        }

        public IEnumerable<TemplateDirectory> GetDirectories(ITemplate template, IEnumerable<ITemplate> templates)
        {
            var nearestDirectories = getNearestDirectories(template);
            foreach (var directory in nearestDirectories)
            {
                yield return directory;
            }
            if (template.FromHost())
            {
                yield break;
            }
            var hostSharedDirectories = getHostSharedDirectoriess(templates);
            foreach (var directory in hostSharedDirectories)
            {
                yield return directory;
            }
        }

        private IEnumerable<TemplateDirectory> getHostSharedDirectoriess(IEnumerable<ITemplate> templates)
        {
            var hostRoot = templates.ByOrigin(FubuSparkConstants.HostOrigin).FirstValue(x => x.RootPath);
            return getSharedPaths(hostRoot).Select(sharedFolder => new TemplateDirectory { Path = sharedFolder, IsShared = true });
        }

        private IEnumerable<TemplateDirectory> getNearestDirectories(ITemplate template)
        {
            var ancestors = getAncestors(template.FilePath, template.RootPath);
            foreach (var directory in ancestors)
            {
                var directoryName = new DirectoryInfo(directory).Name;
                var isShared = _sharedFolderNames.Contains(directoryName);

                yield return new TemplateDirectory { Path = directory, IsShared = isShared };

                foreach (var sharedFolder in getSharedPaths(directory))
                {
                    yield return new TemplateDirectory { Path = sharedFolder, IsShared = true };
                }
            }
        }

        private IEnumerable<string> getSharedPaths(string directory)
        {
            return _sharedFolderNames.Select(sharedFolder => Path.Combine(directory, sharedFolder));
        }
        private static IEnumerable<string> getAncestors(string filePath, string root)
        {
            string directory;
            do
            {
                directory = Path.GetDirectoryName(filePath);
                if (directory == null) break;
                yield return directory;
            } while (directory.IsNotEmpty() && directory.PathRelativeTo(root).IsNotEmpty());
        }
    }

}