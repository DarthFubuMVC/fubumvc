using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration;
using FubuMVC.Core.View;
using System.Linq;
using Spark;
using Spark.FileSystem;

namespace FubuMVC.View.Spark
{
    public class SparkViewFacility : IViewFacility
    {
        private readonly IViewFolder _viewFolder;

        public SparkViewFacility(IViewFolder viewFolder)
        {
            _viewFolder = viewFolder;
        }

        public SparkViewFacility() : this(new VirtualPathProviderViewFolder("~/Views")){}

        public IEnumerable<IViewToken> FindViews(TypePool types)
        {
            var views = _viewFolder.ListViews("");

            return views.Select(path => new SparkViewToken(path)).Cast<IViewToken>();
        }
    }
}
