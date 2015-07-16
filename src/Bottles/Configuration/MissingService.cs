using System;
using FubuCore;

namespace Bottles.Configuration
{
    public class MissingService : BottleConfigurationError
    {
        private readonly Type _serviceType;

        public MissingService(Type serviceType)
        {
            _serviceType = serviceType;
        }

        public Type ServiceType
        {
            get { return _serviceType; }
        }

        public override string ToString()
        {
            return "Missing Service: {0}".ToFormat(_serviceType.FullName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (MissingService)) return false;
            return Equals((MissingService) obj);
        }

        public bool Equals(MissingService other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._serviceType, _serviceType);
        }

        public override int GetHashCode()
        {
            return _serviceType.GetHashCode();
        }
    }
}