using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;

namespace FubuMVC.Core.Registration.DSL
{
    // TODO: Needs test
    public class ExplicitRouteConfiguration : IConfigurationAction
    {
        private readonly IRouteDefinition _route;
        private BehaviorNode _topBehavior;

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
            graph.BehaviorFor(_route).Append(_topBehavior);
            //graph.Observer.RecordStatus("Adding explicit route {0}".ToFormat(_route));
        }


        public ChainedBehaviorExpression Chain()
        {
            return new ChainedBehaviorExpression(node => _topBehavior = node);
        }
    }


    public class ExplicitRouteConfiguration<T> : ExplicitRouteConfiguration
    {
        public ExplicitRouteConfiguration(string pattern)
            : base(RouteBuilder.Build<T>(pattern))
        {
        }
    }
}