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
        private readonly ISharedDirectoryProvider _sharedDirectoryProvider;

        public SharedTemplateLocator() : this(new SharedDirectoryProvider()) {}
        public SharedTemplateLocator(ISharedDirectoryProvider sharedDirectoryProvider)
        {
            _sharedDirectoryProvider = sharedDirectoryProvider;
        }

        public ITemplate LocateTemplate(string name, ITemplate fromTemplate, IEnumerable<ITemplate> templates)
        {
            var reachables = _sharedDirectoryProvider.GetDirectories(fromTemplate, templates);
            return templates.ByName(name).InDirectories(reachables).FirstOrDefault();
        }
    }
}