using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore;
using FubuCore.Reflection;

namespace FubuMVC.Core.Localization
{
    public class PropertyToken
    {
        public static PropertyToken For<T>(Expression<Func<T, object>> expression)
        {
            var property = ReflectionHelper.GetProperty(expression);

            return new PropertyToken(property);
        }

        private readonly IDictionary<string, string> _defaultHeaders = new Dictionary<string, string>();
        private string _header;

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
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }

            property.ForAttribute<HeaderTextAttribute>(att =>
            {
                if (att.Culture.IsEmpty())
                {
                    _header = att.Text;
                }
                else
                {
                    _defaultHeaders.Add(att.Culture, att.Text);
                }
            });

            ParentType = property.DeclaringType;
            ParentTypeName = property.DeclaringType.FullName;
            PropertyName = property.Name;
        }

        public string DefaultHeaderText(CultureInfo culture)
        {
            if (_defaultHeaders.ContainsKey(culture.Name))
            {
                return _defaultHeaders[culture.Name];
            }

            return null;
        }

        public string Header
        {
            get { return _header; }
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
            if (obj.GetType() != typeof(PropertyToken)) return false;
            return Equals((PropertyToken)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((ParentTypeName != null ? ParentTypeName.GetHashCode() : 0) * 397) ^
                       (PropertyName != null ? PropertyName.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return LocalizationManager.GetHeader(this);
        }

        public string FindDefaultHeader(CultureInfo culture)
        {
            if (ParentType == null) return null;

            PropertyInfo property = ParentType.GetProperty(PropertyName);
            string header = null;

            property.ForAttribute<HeaderTextAttribute>(att =>
            {
                if (att.Culture == culture.Name)
                {
                    header = att.Text;
                }
            });

            return header;
        }
        
        // SAMPLE: PropertyTokenKey
        public virtual string StringTokenKey
        {
            get
            {
                return "{0}:{1}:Header".ToFormat(ParentTypeName, PropertyName);
            }
        }
        // ENDSAMPLE
    }
}