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
        private readonly ISparkItemBuilder _itemBuilder;
        public SparkViewFacility(ISparkItemBuilder itemBuilder)
        {
            _itemBuilder = itemBuilder;
        }

        public IEnumerable<IViewToken> FindViews(TypePool types, BehaviorGraph graph)
        {
            return _itemBuilder.BuildItems(types, graph)
                .Where(x => x.HasViewModel())
                .Select(item => new SparkViewToken(item));
        }

        public BehaviorNode CreateViewNode(Type type)
        {
            throw new NotImplementedException();
        }
    }
}