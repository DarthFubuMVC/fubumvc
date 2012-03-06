using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.View.Model;

namespace FubuMVC.Razor.RazorModel
{
    public interface ISharedTemplateLocator
    {
        IRazorTemplate LocateMaster(string masterName, IRazorTemplate fromTemplate, ITemplateRegistry<IRazorTemplate> templateRegistry);
    }

    public class SharedTemplateLocator : ISharedTemplateLocator
    {
        private readonly ITemplateDirectoryProvider _provider;

        public SharedTemplateLocator() : this(new TemplateDirectoryProvider()) { }
        public SharedTemplateLocator(ITemplateDirectoryProvider provider)
        {
            _provider = provider;
        }

        public IRazorTemplate LocateMaster(string masterName, IRazorTemplate fromTemplate, ITemplateRegistry<IRazorTemplate> templateRegistry)
        {
            return locateTemplates(masterName, fromTemplate, templateRegistry, true)
                .Where(x => x.IsRazorView())
                .FirstOrDefault();
        }

        private IEnumerable<IRazorTemplate> locateTemplates(string name, IRazorTemplate fromTemplate, ITemplateRegistry<IRazorTemplate> templateRegistry, bool sharedsOnly)
        {
            var directories = sharedsOnly 
                ? _provider.SharedPathsOf(fromTemplate, templateRegistry) 
                : _provider.ReachablesOf(fromTemplate, templateRegistry);

            return templateRegistry.ByNameUnderDirectories(name, directories);
        }
    }
}