using System;
using FubuCore.Descriptions;

namespace FubuMVC.Core.Registration.Diagnostics
{
    public class VaryByAdded : NodeEvent, DescribesItself
    {
        private readonly Type _varyByType;

        public VaryByAdded(Type varyByType)
        {
            _varyByType = varyByType;
        }

        public void Describe(Description description)
        {
            description.Title = "Added VaryBy: " + _varyByType.Name;
        }

        public Type VaryByType
        {
            get { return _varyByType; }
        }

        protected bool Equals(VaryByAdded other)
        {
            return Equals(_varyByType, other._varyByType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((VaryByAdded) obj);
        }

        public override int GetHashCode()
        {
            return (_varyByType != null ? _varyByType.GetHashCode() : 0);
        }
    }
}