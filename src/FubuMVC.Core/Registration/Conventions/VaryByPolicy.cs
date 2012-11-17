using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore.Descriptions;
using FubuMVC.Core.Caching;

namespace FubuMVC.Core.Registration.Conventions
{

    // TODO -- add this as an option to Policy(?)
    [CanBeMultiples, ConfigurationType(ConfigurationType.Conneg)]
    public class VaryByPolicy : IConfigurationAction, DescribesItself
    {
        private readonly IList<Action<OutputCachingNode>> _modifications = new List<Action<OutputCachingNode>>();
        private readonly StringWriter _description = new StringWriter();

        public VaryByPolicy Apply<T>() where T : IVaryBy
        {
            _modifications.Add(node => node.Apply<T>());
            _description.WriteLine("Add " + typeof(T).Name);

            return this;
        }

        public void Configure(BehaviorGraph graph)
        {
            var nodes = graph.Behaviors.SelectMany(x => x).OfType<OutputCachingNode>();
            nodes.Each(node => _modifications.Each(x => x(node))); 
        }

        public void Describe(Description description)
        {
            description.Title = "Applies VaryBy rules to output caching nodes";
            description.ShortDescription = _description.ToString();
        }
    }
}