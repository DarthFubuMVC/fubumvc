using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Resources.Conneg.New;

namespace FubuMVC.Media
{
    public class ConnegGraph
    {
        private readonly Cache<Type, IList<InputNode>> _inputNodes =
            new Cache<Type, IList<InputNode>>(t => new List<InputNode>());

        private readonly Cache<Type, IList<OutputNode>> _outputNodes =
            new Cache<Type, IList<OutputNode>>(t => new List<OutputNode>());

        public ConnegGraph(BehaviorGraph graph)
        {
            _inputNodes.OnMissing =
                type => graph.Behaviors.Where(x => x.InputType() == type).Select(x => x.Input).ToList();
            _outputNodes.OnMissing =
                type => graph.Behaviors.Where(x => x.ActionOutputType() == type).Select(x => x.Output).ToList();
        }

        public IEnumerable<OutputNode> OutputNodesFor<T>()
        {
            return _outputNodes[typeof (T)];
        }

        public IEnumerable<OutputNode> OutputNodesFor(Type type)
        {
            return _outputNodes[type];
        }

        public IEnumerable<InputNode> InputNodesFor<T>()
        {
            return _inputNodes[typeof (T)];
        }

        public IEnumerable<OutputNode> OutputNodesThatCanBeCastTo(Type interfaceType)
        {
            return _outputNodes.GetAllKeys()
                .Where(x => x.CanBeCastTo(interfaceType))
                .SelectMany(x => _outputNodes[x]);
        }
    }
}