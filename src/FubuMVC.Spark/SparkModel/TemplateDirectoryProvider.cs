using System.Collections.Generic;
using System.IO;
using FubuCore;

namespace FubuMVC.Spark.SparkModel
{
    public interface ITemplateDirectoryProvider
    {
        IEnumerable<string> SharedPathsOf(ITemplate template, IEnumerable<ITemplate> templates);
        IEnumerable<string> ReachablesOf(ITemplate template, IEnumerable<ITemplate> templates);
    }

    public class TemplateDirectoryProvider : ITemplateDirectoryProvider
    {
        private readonly ISharedPathBuilder _builder;

        public TemplateDirectoryProvider() : this(new SharedPathBuilder()) { }
        public TemplateDirectoryProvider(ISharedPathBuilder builder)
        {
            _builder = builder;
        }

        public IEnumerable<string> SharedPathsOf(ITemplate template, IEnumerable<ITemplate> templates)
        {
            return getDirectories(template, templates, false);
        }

        public IEnumerable<string> ReachablesOf(ITemplate template, IEnumerable<ITemplate> templates)
        {
            return getDirectories(template, templates, true);
        }

        private IEnumerable<string> getDirectories(ITemplate template, IEnumerable<ITemplate> templates, bool includeDirectAncestor)
        {
            foreach (var directory in _builder.BuildBy(template.FilePath, template.RootPath, includeDirectAncestor))
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
            if(includeDirectAncestor)
            {
                yield return hostRoot;
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