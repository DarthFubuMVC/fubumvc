using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.View;

namespace FubuMVC.Spark
{
    public class SparkViewFacility : IViewFacility
    {
        private readonly IViewTokenSource _tokenSource;
        public SparkViewFacility(IViewTokenSource tokenSource)
        {
            _tokenSource = tokenSource;
        }

        public IEnumerable<IViewToken> FindViews(TypePool types, BehaviorGraph graph)
        {
            return _tokenSource.FindFrom(graph.Actions());
        }

        public BehaviorNode CreateViewNode(Type type)
        {
            throw new NotImplementedException();
        }
    }
}