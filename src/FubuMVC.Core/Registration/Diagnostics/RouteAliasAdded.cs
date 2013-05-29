using FubuCore.Descriptions;
using FubuMVC.Core.Registration.Routes;

namespace FubuMVC.Core.Registration.Diagnostics
{
    [Title("Route Alias Added")]
    public class RouteAliasAdded : NodeEvent
    {
        private readonly IRouteDefinition _definition;

        public RouteAliasAdded(IRouteDefinition definition)
        {
            _definition = definition;
        }

        public IRouteDefinition Definition
        {
            get { return _definition; }
        }

        public bool Equals(RouteAliasAdded other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._definition, _definition);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(RouteAliasAdded)) return false;
            return Equals((RouteAliasAdded)obj);
        }

        public override int GetHashCode()
        {
            return (_definition != null ? _definition.GetHashCode() : 0);
        }
    }
}