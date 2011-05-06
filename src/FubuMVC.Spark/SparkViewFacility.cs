using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.View;
using FubuMVC.Spark.SparkModel;

namespace FubuMVC.Spark
{
    public class SparkViewFacility : IViewFacility
    {
        private readonly ISparkItemComposer _composer;
        public SparkViewFacility(ISparkItemComposer composer)
        {
            _composer = composer;
        }

        public IEnumerable<IViewToken> FindViews(TypePool types, BehaviorGraph graph)
        {
            return _composer.ComposeViews(types)
                .Where(x => x.HasViewModel())
                .Select(item => new SparkViewToken(item));
        }

        public BehaviorNode CreateViewNode(Type type)
        {
            return null;
        }
    }
}