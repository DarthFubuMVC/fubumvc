using System;
using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;

namespace FubuMVC.Core.Registration.DSL
{
    public class ExplicitRouteConfiguration : IConfigurationAction
    {
        private readonly IRouteDefinition _route;
        private readonly IList<BehaviorNode> _nodes = new List<BehaviorNode>();

        public ExplicitRouteConfiguration(IRouteDefinition route)
        {
            _route = route;
        }

        public ExplicitRouteConfiguration(string pattern)
        {
            _route = new RouteDefinition(pattern);
        }

        void IConfigurationAction.Configure(BehaviorGraph graph)
        {
            var chain = graph.BehaviorFor(_route);
            _nodes.Each(chain.AddToEnd);
            
            //graph.Observer.RecordStatus("Adding explicit route {0}".ToFormat(_route));
        }


        public ChainedBehaviorExpression Chain()
        {
            return new ChainedBehaviorExpression(node => _nodes.Add(node));
        }
    }

}