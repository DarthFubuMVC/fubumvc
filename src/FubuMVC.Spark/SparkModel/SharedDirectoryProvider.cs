using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Spark.SparkModel
{
    // NOTE: RECONSIDER THIS, IT HAS BECOME A HELPER
    public interface ISharedDirectoryProvider
    {
        IEnumerable<string> GetDirectories(ITemplate template, IEnumerable<ITemplate> templates);
    }

    public class SharedDirectoryProvider : ISharedDirectoryProvider
    {
        private readonly IReachableDirectoryLocator _locator;

        public SharedDirectoryProvider() : this(new ReachableDirectoryLocator()) {}
        public SharedDirectoryProvider(IReachableDirectoryLocator locator)
        {
            _locator = locator;
        }

        public IEnumerable<string> GetDirectories(ITemplate template, IEnumerable<ITemplate> templates)
        {
            return _locator.GetDirectories(template, templates)
                .Where(x => x.IsShared)
                .Select(x => x.Path);
        }
    }
}