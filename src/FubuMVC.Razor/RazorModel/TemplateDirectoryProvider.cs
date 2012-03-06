using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuMVC.Core.View.Model;

namespace FubuMVC.Razor.RazorModel
{
    public interface ITemplateDirectoryProvider
    {
        IEnumerable<string> SharedPathsOf(IRazorTemplate template, ITemplateRegistry<IRazorTemplate> templateRegistry);
        IEnumerable<string> ReachablesOf(IRazorTemplate template, ITemplateRegistry<IRazorTemplate> templateRegistry);
    }

    public class TemplateDirectoryProvider : ITemplateDirectoryProvider
    {
        private readonly ISharedPathBuilder _builder;

        public TemplateDirectoryProvider() : this(new SharedPathBuilder()) { }
        public TemplateDirectoryProvider(ISharedPathBuilder builder)
        {
            _builder = builder;
        }

        public IEnumerable<string> SharedPathsOf(IRazorTemplate template, ITemplateRegistry<IRazorTemplate> templateRegistry)
        {
            return getDirectories(template, templateRegistry, false);
        }

        public IEnumerable<string> ReachablesOf(IRazorTemplate template, ITemplateRegistry<IRazorTemplate> templateRegistry)
        {
            return getDirectories(template, templateRegistry, true);
        }

        private IEnumerable<string> getDirectories(IRazorTemplate template, ITemplateRegistry<IRazorTemplate> templateRegistry, bool includeDirectAncestor)
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