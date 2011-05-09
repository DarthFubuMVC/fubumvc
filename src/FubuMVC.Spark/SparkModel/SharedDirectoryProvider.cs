using System.Collections.Generic;
using System.IO;
using FubuCore;

namespace FubuMVC.Spark.SparkModel
{
    public interface ISharedDirectoryProvider
    {
        IEnumerable<string> GetDirectories(ITemplate template, IEnumerable<ITemplate> templates);
    }

    public class SharedDirectoryProvider : ISharedDirectoryProvider
    {
        private readonly ISharedPathBuilder _builder;

        public SharedDirectoryProvider() : this(new SharedPathBuilder()) {}
        public SharedDirectoryProvider(ISharedPathBuilder builder)
        {
            _builder = builder;
        }

        public IEnumerable<string> GetDirectories(ITemplate template, IEnumerable<ITemplate> templates)
        {
            foreach (var directory in _builder.BuildFrom(template.FilePath, template.RootPath))
            {
                yield return directory;
            }
			
            if (template.Origin == FubuSparkConstants.HostOrigin)
            {
                yield break;
            }
			
            var hostRoot = findHostRoot(templates);
            if (hostRoot.IsEmpty())
            {
                yield break;
            }

            foreach (var sharedFolder in _builder.SharedFolderNames)
            {
                yield return Path.Combine(hostRoot, sharedFolder);
            }
        }

        private static string findHostRoot(IEnumerable<ITemplate> templates)
        {
            return templates.ByOrigin(FubuSparkConstants.HostOrigin).FirstValue(x => x.RootPath);
        }
    }
}