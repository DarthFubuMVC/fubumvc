using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Spark.SparkModel
{
    public interface ITemplateLocator
    {
        ITemplate LocateMaster(string masterName, ITemplate fromTemplate, ITemplates templates);
        IEnumerable<ITemplate> LocateBindings(string bindingName, ITemplate fromTemplate, ITemplates templates); 
    }

    public class TemplateLocator : ITemplateLocator
    {
        private readonly ITemplateDirectoryProvider _provider;

        public TemplateLocator() : this(new TemplateDirectoryProvider()) { }
        public TemplateLocator(ITemplateDirectoryProvider provider)
        {
            _provider = provider;
        }

        public ITemplate LocateMaster(string masterName, ITemplate fromTemplate, ITemplates templates)
        {
            return locateTemplates(masterName, fromTemplate, templates, true)
                .Where(x => x.IsSparkView())
                .FirstOrDefault();
        }

        public IEnumerable<ITemplate> LocateBindings(string bindingName, ITemplate fromTemplate, ITemplates templates)
        {
            return locateTemplates(bindingName, fromTemplate, templates, false)
                .Where(x => x.IsXml());
        }

        private IEnumerable<ITemplate> locateTemplates(string name, ITemplate fromTemplate, ITemplates templates, bool sharedsOnly)
        {
            var directories = sharedsOnly 
                ? _provider.SharedPathsOf(fromTemplate, templates) 
                : _provider.ReachablesOf(fromTemplate, templates);
            return templates.ByNameAndDirectories(name, directories);
        }
    }
}