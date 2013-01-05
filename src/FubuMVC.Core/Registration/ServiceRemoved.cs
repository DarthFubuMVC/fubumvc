using System;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Core.Registration
{
    public class ServiceRemoved : ServiceEvent
    {
        public ServiceRemoved(Type serviceType, ObjectDef def) : base(serviceType, def)
        {
        }

        public bool Equals(ServiceRemoved other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.ServiceType, ServiceType) && Equals(other.Def, Def);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ServiceRemoved)) return false;
            return Equals((ServiceRemoved) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((ServiceType != null ? ServiceType.GetHashCode() : 0) * 397) ^ (Def != null ? Def.GetHashCode() : 0);
            }
        }

        protected override string title()
        {
            return "Service Removed";
        }
    }
}