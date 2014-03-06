using System.Collections.Generic;
using System.Linq;
using FubuCore;

namespace FubuMVC.Core.View.Model
{
    public interface ISharedTemplateLocator<T> where T : ITemplateFile
    {
        T LocateMaster(string masterName, T fromTemplate);
        T LocatePartial(string partialName, T fromTemplate);
    }

    public class SharedTemplateLocator<T> : ISharedTemplateLocator<T> where T : ITemplateFile
    {
        private readonly ITemplateDirectoryProvider<T> _provider;
        private readonly ITemplateRegistry<T> _templates;

        public SharedTemplateLocator(ITemplateDirectoryProvider<T> provider, ITemplateRegistry<T> templates)
        {
            _provider = provider;
            _templates = templates;
        }

        public T LocateMaster(string masterName, T fromTemplate)
        {
            return locate(masterName, fromTemplate);
        }

        public T LocatePartial(string partialName, T fromTemplate)
        {
            return locate("_{0}".ToFormat(partialName), fromTemplate);
        }

        private T locate(string name, T fromTemplate)
        {
            return locateTemplates(name, fromTemplate, true)
                .FirstOrDefault();
        }

        protected IEnumerable<T> locateTemplates(string name, T fromTemplate, bool sharedsOnly)
        {
            var directories = sharedsOnly 
                ? _provider.SharedPathsOf(fromTemplate) 
                : _provider.ReachablesOf(fromTemplate);

            return _templates.ByNameUnderDirectories(name, directories);
        }
    }

}