using System;
using System.Reflection;

namespace FubuCore.Localization
{
    public class PropertyToken
    {
        public PropertyToken()
        {
        }

        public PropertyToken(string parentType, string propertyName)
        {
            ParentTypeName = parentType;
            PropertyName = propertyName;
        }

        public PropertyToken(PropertyInfo property)
        {
            ParentTypeName = property.DeclaringType.FullName;
            PropertyName = property.Name;
        }

        public string ParentTypeName { get; set; }
        public Type ParentType { get; set; }
        public string PropertyName { get; set; }

        public bool Equals(PropertyToken obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj.ParentTypeName, ParentTypeName) && Equals(obj.PropertyName, PropertyName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (PropertyToken)) return false;
            return Equals((PropertyToken) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((ParentTypeName != null ? ParentTypeName.GetHashCode() : 0)*397) ^
                       (PropertyName != null ? PropertyName.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return Localizer.HeaderTextFor(this);
        }

        public string ToString(params object[] parameters)
        {
            return ToString().ToFormat(parameters);
        }
    }
}