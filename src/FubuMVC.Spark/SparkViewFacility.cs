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
        private readonly SparkItems _sparkItems;

        public SparkViewFacility(ISparkItemBuilder itemBuilder, SparkItems sparkItems)
        {
            _itemBuilder = itemBuilder;
            _sparkItems = sparkItems;
        }

        public IEnumerable<IViewToken> FindViews(TypePool types, BehaviorGraph graph)
        {
            _sparkItems.AddRange(_itemBuilder.BuildItems(types, graph));
            return _sparkItems.Where(x => x.HasViewModel())
                .Select(item => new SparkViewToken(item));
        }

        public BehaviorNode CreateViewNode(Type type)
        {
            throw new NotImplementedException();
        }
    }
}