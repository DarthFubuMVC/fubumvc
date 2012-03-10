using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.View.Model;

namespace FubuMVC.Razor.RazorModel
{
    public interface ISharedTemplateLocator
    {
        IRazorTemplate LocateMaster(string masterName, IRazorTemplate fromTemplate);
    }

    public class SharedTemplateLocator : ISharedTemplateLocator
    {
        private readonly ITemplateDirectoryProvider<IRazorTemplate> _provider;
        private readonly ITemplateRegistry<IRazorTemplate> _templates;

        public SharedTemplateLocator(ITemplateDirectoryProvider<IRazorTemplate> provider, ITemplateRegistry<IRazorTemplate> templates)
        {
            _provider = provider;
            _templates = templates;
        }

        public IRazorTemplate LocateMaster(string masterName, IRazorTemplate fromTemplate)
        {
            return locateTemplates(masterName, fromTemplate, true)
                .Where(x => x.IsRazorView())
                .FirstOrDefault();
        }

        private IEnumerable<IRazorTemplate> locateTemplates(string name, IRazorTemplate fromTemplate, bool sharedsOnly)
        {
            var directories = sharedsOnly 
                ? _provider.SharedPathsOf(fromTemplate) 
                : _provider.ReachablesOf(fromTemplate);

            return _templates.ByNameUnderDirectories(name, directories);
        }
    }
}