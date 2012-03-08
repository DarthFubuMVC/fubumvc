using FubuMVC.Core.Registration.Routes;

namespace FubuMVC.Core.Registration.Diagnostics
{
    public class RouteDefined : NodeEvent
    {
        private readonly IRouteDefinition _definition;

        public RouteDefined(IRouteDefinition definition)
        {
            _definition = definition;
        }

        public IRouteDefinition Definition
        {
            get { return _definition; }
        }

        public bool Equals(RouteDefined other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._definition, _definition);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (RouteDefined)) return false;
            return Equals((RouteDefined) obj);
        }

        public override int GetHashCode()
        {
            return (_definition != null ? _definition.GetHashCode() : 0);
        }
    }
}