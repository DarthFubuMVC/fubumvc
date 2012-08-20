using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Caching;

namespace FubuMVC.Core.Registration.Conventions
{
    [CanBeMultiples, ConfigurationType(ConfigurationType.Reordering)]
    public class VaryByPolicy : IConfigurationAction
    {
        private readonly IList<Action<OutputCachingNode>> _modifications = new List<Action<OutputCachingNode>>();

        public VaryByPolicy Apply<T>() where T : IVaryBy
        {
            _modifications.Add(node => node.Apply<T>());

            return this;
        }

        public void Configure(BehaviorGraph graph)
        {
            var nodes = graph.Behaviors.SelectMany(x => x).OfType<OutputCachingNode>();
            nodes.Each(node => _modifications.Each(x => x(node))); 
        }
    }
}