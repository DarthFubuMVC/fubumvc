using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FubuMVC.Spark.SparkModel
{
    public interface ITemplateDirectoryProvider
    {
        IEnumerable<string> SharedPathsOf(ITemplate template, ITemplateRegistry templateRegistry);
        IEnumerable<string> ReachablesOf(ITemplate template, ITemplateRegistry templateRegistry);
    }

    public class TemplateDirectoryProvider : ITemplateDirectoryProvider
    {
        private readonly ISharedPathBuilder _builder;

        public TemplateDirectoryProvider() : this(new SharedPathBuilder()) { }
        public TemplateDirectoryProvider(ISharedPathBuilder builder)
        {
            _builder = builder;
        }

        public IEnumerable<string> SharedPathsOf(ITemplate template, ITemplateRegistry templateRegistry)
        {
            return getDirectories(template, templateRegistry, false);
        }

        public IEnumerable<string> ReachablesOf(ITemplate template, ITemplateRegistry templateRegistry)
        {
            return getDirectories(template, templateRegistry, true);
        }

        private IEnumerable<string> getDirectories(ITemplate template, ITemplateRegistry templateRegistry, bool includeDirectAncestor)
        {
            foreach (var directory in _builder.BuildBy(template.FilePath, template.RootPath, includeDirectAncestor))
            {
                yield return directory;
            }

            if (template.FromHost())
            {
                yield break;
            }

            var hostTemplate = templateRegistry.FromHost().FirstOrDefault();
            if (hostTemplate == null)
            {
                yield break;
            }

            var hostRoot = hostTemplate.RootPath;
            if (includeDirectAncestor)
            {
                yield return hostRoot;
            }

            foreach (var sharedFolder in _builder.SharedFolderNames)
            {
                yield return Path.Combine(hostRoot, sharedFolder);
            }
        }
    }
}