using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;

namespace FubuMVC.Spark.SparkModel
{
    public interface ITemplateDirectoryProvider
    {
        IEnumerable<string> SharedPathsOf(ITemplate template, ITemplates templates);
        IEnumerable<string> ReachablesOf(ITemplate template, ITemplates templates);
    }

    public class TemplateDirectoryProvider : ITemplateDirectoryProvider
    {
        private readonly ISharedPathBuilder _builder;

        public TemplateDirectoryProvider() : this(new SharedPathBuilder()) { }
        public TemplateDirectoryProvider(ISharedPathBuilder builder)
        {
            _builder = builder;
        }

        public IEnumerable<string> SharedPathsOf(ITemplate template, ITemplates templates)
        {
            return getDirectories(template, templates, false);
        }

        public IEnumerable<string> ReachablesOf(ITemplate template, ITemplates templates)
        {
            return getDirectories(template, templates, true);
        }

        private IEnumerable<string> getDirectories(ITemplate template, ITemplates templates, bool includeDirectAncestor)
        {
            foreach (var directory in _builder.BuildBy(template.FilePath, template.RootPath, includeDirectAncestor))
            {
                yield return directory;
            }

            if (template.Origin == FubuSparkConstants.HostOrigin)
            {
                yield break;
            }
            var hostTemplate = templates.ByOrigin(FubuSparkConstants.HostOrigin).FirstOrDefault();
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