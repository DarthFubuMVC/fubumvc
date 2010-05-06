using System;
using System.Reflection;
using FubuCore.Util;

namespace FubuCore.Localization
{
    public class StringToken
    {
        protected StringToken(string defaultText)
        {
            DefaultText = defaultText;
        }

        public string DefaultText { get; set; }

        public string Key { get; protected set; }

        public static StringToken FromKeyString(string key)
        {
            return new StringToken(null)
            {
                DefaultText = null
            };
        }

        public override string ToString()
        {
            return Localizer.TextFor(this);
        }
    }



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

    public class NulloLocalizationProvider : ILocalizationProvider
    {
        public string TextFor(StringToken token)
        {
            return token.DefaultText ?? token.Key;
        }

        public LocalizedHeader HeaderFor(PropertyInfo property)
        {
            return new LocalizedHeader()
            {
                Heading = property.Name
            };
        }

        public LocalizedHeader HeaderFor(PropertyToken property)
        {
            return new LocalizedHeader()
            {
                Heading = property.PropertyName
            };
        }
    }


    public interface ILocalizationProvider
    {
        string TextFor(StringToken token);
        LocalizedHeader HeaderFor(PropertyInfo property);
        LocalizedHeader HeaderFor(PropertyToken property);
    }

    public class LocalizedHeader
    {
        private readonly Cache<string, string> _properties = new Cache<string, string>();

        public string Heading { get; set; }

        public string this[string name]
        {
            get
            {
                return _properties[name];
            }
            set
            {
                _properties[name] = value;
            }
        }
    }

    public static class Localizer
    {
        private static Func<ILocalizationProvider> _providerSource;

        public static Func<ILocalizationProvider> ProviderSource
        {
            set
            {
                _providerSource = value;
            }
        }

        public static ILocalizationProvider Provider
        {
            get
            {
                return _providerSource();
            }
        }

        public static string TextFor(StringToken token)
        {
            return _providerSource().TextFor(token);
        }

        public static string HeaderTextFor(PropertyInfo property)
        {
            return HeaderFor(property).Heading;
        }

        public static string HeaderTextFor(PropertyToken property)
        {
            return HeaderFor(property).Heading;
        }

        public static LocalizedHeader HeaderFor(PropertyInfo property)
        {
            return _providerSource().HeaderFor(property);
        }

        public static LocalizedHeader HeaderFor(PropertyToken property)
        {
            return _providerSource().HeaderFor(property);
        }
    }
}