using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Spark.SparkModel
{
    public interface ITemplateLocator
    {
        IEnumerable<ITemplate> LocateSharedTemplates(string name, ITemplate fromTemplate, IEnumerable<ITemplate> templates);
        ITemplate LocateMaster(string masterName, ITemplate fromTemplate, IEnumerable<ITemplate> templates);
        IEnumerable<ITemplate> LocateBindings(string bindingName, ITemplate fromTemplate, IEnumerable<ITemplate> templates); 
    }

    public class TemplateLocator : ITemplateLocator
    {
        private readonly ITemplateDirectoryProvider _provider;

        public TemplateLocator() : this(new TemplateDirectoryProvider()) { }
        public TemplateLocator(ITemplateDirectoryProvider provider)
        {
            _provider = provider;
        }

        // NOTE : Only used by Tests. Let's reconsider the test for TemplateLocator and kill of this method.
        public IEnumerable<ITemplate> LocateSharedTemplates(string name, ITemplate fromTemplate, IEnumerable<ITemplate> templates)
        {
            return locateTemplates(name, fromTemplate, templates, true);
        }

        public ITemplate LocateMaster(string masterName, ITemplate fromTemplate, IEnumerable<ITemplate> templates)
        {
            return locateTemplates(masterName, fromTemplate, templates, true)
                .Where(x => x.IsSparkView())
                .FirstOrDefault();
        }

        public IEnumerable<ITemplate> LocateBindings(string bindingName, ITemplate fromTemplate, IEnumerable<ITemplate> templates)
        {
            return locateTemplates(bindingName, fromTemplate, templates, false)
                .Where(x => x.IsXml());
        }

        private IEnumerable<ITemplate> locateTemplates(string name, ITemplate fromTemplate, IEnumerable<ITemplate> templates, bool sharedsOnly)
        {
            var directories = sharedsOnly 
                ? _provider.SharedPathsOf(fromTemplate, templates) 
                : _provider.ReachablesOf(fromTemplate, templates);
            
            return templates.ByName(name).InDirectories(directories);
        }
    }
}