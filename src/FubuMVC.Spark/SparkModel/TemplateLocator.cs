using System.Collections.Generic;

namespace FubuMVC.Spark.SparkModel
{
    public interface ITemplateLocator
    {
        IEnumerable<ITemplate> LocateSharedTemplates(string name, ITemplate fromTemplate, IEnumerable<ITemplate> templates);
        IEnumerable<ITemplate> LocateReachableTemplates(string name, ITemplate fromTemplate, IEnumerable<ITemplate> templates);
    }

    public class TemplateLocator : ITemplateLocator
    {
        private readonly ITemplateDirectoryProvider _provider;

        public TemplateLocator() : this(new TemplateDirectoryProvider()) { }
        public TemplateLocator(ITemplateDirectoryProvider provider)
        {
            _provider = provider;
        }

        public IEnumerable<ITemplate> LocateSharedTemplates(string name, ITemplate fromTemplate, IEnumerable<ITemplate> templates)
        {
            return locateTemplates(name, fromTemplate, templates, true);
        }

        public IEnumerable<ITemplate> LocateReachableTemplates(string name, ITemplate fromTemplate, IEnumerable<ITemplate> templates)
        {
            return locateTemplates(name, fromTemplate, templates, false);
        }

        private IEnumerable<ITemplate> locateTemplates(string name, ITemplate fromTemplate, IEnumerable<ITemplate> templates, bool sharedsOnly)
        {
            var reachables = sharedsOnly
                                 ? _provider.SharedPathsOf(fromTemplate, templates)
                                 : _provider.ReachablesOf(fromTemplate, templates);
            return templates.ByName(name).InDirectories(reachables);
        }

    }
}