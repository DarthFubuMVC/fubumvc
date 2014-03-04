using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;
using FubuMVC.Core.View.Model;
using Spark;

namespace FubuMVC.Spark.SparkModel
{
    public class FubuPartialProvider : IPartialProvider
    {
        private readonly ITemplateDirectoryProvider<ITemplate> _directoryProvider;
        private readonly DefaultPartialProvider _defaultPartialProvider;
        private readonly Cache<string, IEnumerable<string>> _partialPathCache;

        public FubuPartialProvider(ITemplateDirectoryProvider<ITemplate> directoryProvider)
        {
            _directoryProvider = directoryProvider;
            _defaultPartialProvider = new DefaultPartialProvider();
            _partialPathCache = new Cache<string, IEnumerable<string>>(getPaths);
        }

        public IEnumerable<string> GetPaths(string viewPath)
        {
            return _partialPathCache[viewPath];
        }

        private IEnumerable<string> getPaths(string viewPath)
        {
            var defaultPaths = _defaultPartialProvider.GetPaths(viewPath);
            var origin = viewPath.GetOrigin();
            var sharedPaths = _directoryProvider.SharedViewPathsForOrigin(origin);
            return defaultPaths.Union(sharedPaths);
        }
    }
}