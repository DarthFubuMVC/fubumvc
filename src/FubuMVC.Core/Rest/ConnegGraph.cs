using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Rest.Conneg;

namespace FubuMVC.Core.Rest
{
    public class ConnegGraph
    {
        private readonly Cache<Type, IList<ConnegInputNode>> _inputNodes =
            new Cache<Type, IList<ConnegInputNode>>(t => new List<ConnegInputNode>());

        private readonly Cache<Type, IList<ConnegOutputNode>> _outputNodes =
            new Cache<Type, IList<ConnegOutputNode>>(t => new List<ConnegOutputNode>());

        public ConnegGraph(BehaviorGraph graph)
        {
            var connegNodes = graph.Behaviors.SelectMany(x => x).Where(x => x is ConnegNode);
            connegNodes.OfType<ConnegInputNode>().GroupBy(x => x.InputType)
                .Each(group => _inputNodes[group.Key].AddRange(group));

            connegNodes.OfType<ConnegOutputNode>().GroupBy(x => x.InputType)
                .Each(group => _outputNodes[group.Key].AddRange(group));
        }

        public IEnumerable<ConnegOutputNode> OutputNodesFor<T>()
        {
            return _outputNodes[typeof (T)];
        }

        public IEnumerable<ConnegInputNode> InputNodesFor<T>()
        {
            return _inputNodes[typeof (T)];
        }
    }
}