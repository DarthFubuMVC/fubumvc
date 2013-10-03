using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Descriptions;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Resources.Conneg;

namespace FubuMVC.Core.Registration.DSL
{
    [ConfigurationType(ConfigurationType.Explicit)]
    [Title("Explicit route definition")]
    public class ExplicitRouteConfiguration : IConfigurationAction
    {
        private readonly IRouteDefinition _route;
        private readonly IList<BehaviorNode> _nodes = new List<BehaviorNode>();
        private readonly IList<Action<BehaviorChain>> _chainConfigs = new List<Action<BehaviorChain>>();

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
            _chainConfigs.Each(x => x(chain));
            
        }


        public ChainedBehaviorExpression Chain()
        {
            return new ChainedBehaviorExpression(this);
        }

        public class ChainedBehaviorExpression
        {
            private readonly ExplicitRouteConfiguration _parent;

            public ChainedBehaviorExpression(ExplicitRouteConfiguration parent)
            {
                _parent = parent;
            }

            public ChainedBehaviorExpression Calls<C>(Expression<Action<C>> expression)
            {
                var method = ReflectionHelper.GetMethod(expression);
                var call = new ActionCall(typeof(C), method);

                _parent._nodes.Add(call);

                return this;
            }

            public ChainedBehaviorExpression AlterChain(Action<BehaviorChain> configure)
            {
                _parent._chainConfigs.Add(configure);
                return this;
            }

            public ChainedBehaviorExpression OutputToJson()
            {
                return AlterChain(x => x.MakeAsymmetricJson());
            }

        }
    }

}