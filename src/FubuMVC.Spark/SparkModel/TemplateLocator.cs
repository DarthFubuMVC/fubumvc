using System.Collections.Generic;

namespace FubuMVC.Spark.SparkModel
{
    public interface ITemplateLocator
    {
        IEnumerable<ITemplate> LocateSharedTemplates(string name, ITemplate fromTemplate, IEnumerable<ITemplate> templates);
        IEnumerable<ITemplate> LocateTemplates(string name, ITemplate fromTemplate, IEnumerable<ITemplate> templates);
    }

    public class TemplateLocator : ITemplateLocator
    {
        private readonly ISharedDirectoryProvider _provider;

        public TemplateLocator() : this(new SharedDirectoryProvider()) { }
        public TemplateLocator(ISharedDirectoryProvider provider)
        {
            _provider = provider;
        }

        public IEnumerable<ITemplate> LocateSharedTemplates(string name, ITemplate fromTemplate, IEnumerable<ITemplate> templates)
        {
            return locateTemplates(name, fromTemplate, templates, false);
        }

        public IEnumerable<ITemplate> LocateTemplates(string name, ITemplate fromTemplate, IEnumerable<ITemplate> templates)
        {
            return locateTemplates(name, fromTemplate, templates, true);
        }

        private IEnumerable<ITemplate> locateTemplates(string name, ITemplate fromTemplate, IEnumerable<ITemplate> templates, bool includeDirectAncestor)
        {
            var reachables = _provider.GetDirectories(fromTemplate, templates, includeDirectAncestor);
            return templates.ByName(name).InDirectories(reachables);
        }
    }
}