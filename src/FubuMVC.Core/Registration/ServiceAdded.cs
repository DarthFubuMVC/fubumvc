using System;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Core.Registration
{
    public class ServiceAdded : ServiceEvent
    {
        public ServiceAdded(Type serviceType, ObjectDef def) : base(serviceType, def)
        {
        }

        protected override string title()
        {
            return "Service Added";
        }

        public bool Equals(ServiceAdded other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.ServiceType, ServiceType) && Equals(other.Def, Def);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ServiceAdded)) return false;
            return Equals((ServiceAdded) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((ServiceType != null ? ServiceType.GetHashCode() : 0) * 397) ^ (Def != null ? Def.GetHashCode() : 0);
            }
        }
    }
}