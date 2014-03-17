using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;
using Spark;

namespace FubuMVC.Spark.SparkModel
{
    public class FubuPartialProvider : IPartialProvider
    {
        private readonly Cache<string, IEnumerable<string>> _partialPathCache;

        public FubuPartialProvider(SparkViewFacility facility)
        {
            var partialProvider = new DefaultPartialProvider();

            var sharedPaths = facility.SharedPaths();

            _partialPathCache = new Cache<string, IEnumerable<string>>(viewPath => sharedPaths.Union(partialProvider.GetPaths(viewPath)));

        }

        public IEnumerable<string> GetPaths(string viewPath)
        {
            return _partialPathCache[viewPath];
        }
    }
}