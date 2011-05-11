using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Spark.SparkModel
{
    // TODO : Below needs changes wrt locating single or many.
    public interface ISharedTemplateLocator
    {
        ITemplate LocateTemplate(string name, ITemplate fromTemplate, IEnumerable<ITemplate> templates);
    }
	
    public class SharedTemplateLocator : ISharedTemplateLocator
    {
        private readonly ISharedDirectoryProvider _provider;

        public SharedTemplateLocator() : this(new SharedDirectoryProvider()) { }
        public SharedTemplateLocator(ISharedDirectoryProvider provider)
        {
            _provider = provider;
        }

        public ITemplate LocateTemplate(string name, ITemplate fromTemplate, IEnumerable<ITemplate> templates)
        {
            var reachables = _provider.GetDirectories(fromTemplate, templates, false);
            return templates.ByName(name).InDirectories(reachables).FirstOrDefault();
        }
    }
}