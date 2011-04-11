using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.View;
using FubuMVC.Spark.Scanning;

namespace FubuMVC.Spark
{
    public class SparkViewFacility : IViewFacility
    {
        private readonly Func<ActionCall, SparkFile> _builder;

        // NOTE: Perhaps a bit more clear with an explicit interface
        public SparkViewFacility(Func<ActionCall, SparkFile> builder)
        {
            _builder = builder;
        }

        public IEnumerable<IViewToken> FindViews(TypePool types, BehaviorGraph graph)
        {
            foreach (var action in graph.Actions())
            {
                var file = _builder(action);
                if (file == null)
                {
                    continue;
                }
                yield return new SparkViewToken(file, action);
            }
        }

        public BehaviorNode CreateViewNode(Type type)
        {
            throw new NotImplementedException();
        }
    }
}