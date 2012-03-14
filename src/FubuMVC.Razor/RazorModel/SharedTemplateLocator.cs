using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.View.Model;

namespace FubuMVC.Razor.RazorModel
{
    public interface ISharedTemplateLocator<T> where T : ITemplateFile
    {
        T LocateMaster(string masterName, T fromTemplate);
    }

    public class SharedTemplateLocator<T> : ISharedTemplateLocator<T> where T : ITemplateFile
    {
        private readonly ITemplateDirectoryProvider<T> _provider;
        private readonly ITemplateRegistry<T> _templates;
        private readonly ITemplateSelector<T> _templateSelector;

        public SharedTemplateLocator(ITemplateDirectoryProvider<T> provider, ITemplateRegistry<T> templates, ITemplateSelector<T> templateSelector)
        {
            _provider = provider;
            _templates = templates;
            _templateSelector = templateSelector;
        }

        public T LocateMaster(string masterName, T fromTemplate)
        {
            return locateTemplates(masterName, fromTemplate, true)
                .Where(x => _templateSelector.IsAppropriate(x))
                .FirstOrDefault();
        }

        private IEnumerable<T> locateTemplates(string name, T fromTemplate, bool sharedsOnly)
        {
            var directories = sharedsOnly 
                ? _provider.SharedPathsOf(fromTemplate) 
                : _provider.ReachablesOf(fromTemplate);

            return _templates.ByNameUnderDirectories(name, directories);
        }
    }

    public interface ITemplateSelector<T>
    {
        bool IsAppropriate(T template);
    }

    public class RazorTemplateSelector : ITemplateSelector<IRazorTemplate>
    {
        public bool IsAppropriate(IRazorTemplate template)
        {
            return template.IsRazorView();
        }
    }
}