using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Spark.SparkModel
{
    public interface ISharedTemplateLocator
    {
        ITemplate LocateMaster(string masterName, ITemplate fromTemplate, ITemplateRegistry templateRegistry);
        IEnumerable<ITemplate> LocateBindings(string bindingName, ITemplate fromTemplate, ITemplateRegistry templateRegistry); 
    }

    public class SharedTemplateLocator : ISharedTemplateLocator
    {
        private readonly ITemplateDirectoryProvider _provider;

        public SharedTemplateLocator() : this(new TemplateDirectoryProvider()) { }
        public SharedTemplateLocator(ITemplateDirectoryProvider provider)
        {
            _provider = provider;
        }

        public ITemplate LocateMaster(string masterName, ITemplate fromTemplate, ITemplateRegistry templateRegistry)
        {
            return locateTemplates(masterName, fromTemplate, templateRegistry, true)
                .Where(x => x.IsSparkView())
                .FirstOrDefault();
        }

        public IEnumerable<ITemplate> LocateBindings(string bindingName, ITemplate fromTemplate, ITemplateRegistry templateRegistry)
        {
            return locateTemplates(bindingName, fromTemplate, templateRegistry, false)
                .Where(x => x.IsXml());
        }

        private IEnumerable<ITemplate> locateTemplates(string name, ITemplate fromTemplate, ITemplateRegistry templateRegistry, bool sharedsOnly)
        {
            var directories = sharedsOnly 
                ? _provider.SharedPathsOf(fromTemplate, templateRegistry) 
                : _provider.ReachablesOf(fromTemplate, templateRegistry);

            return templateRegistry.ByNameUnderDirectories(name, directories);
        }
    }
}