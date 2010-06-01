using System;
using System.Reflection;
using FubuCore.Util;

namespace FubuCore.Localization
{
    public class StringToken
    {
        // Notice that there's no public ctor
        // We use StringToken as a Java style
        // strongly typed enumeration
        protected StringToken(string defaultText)
        {
            DefaultText = defaultText;
        }

        public string DefaultText { get; set; }

        // I'm thinking about a way that the Key
        // gets automatically assigned to the field name  
        // of the holding class 
        public string Key { get; protected set; }

        public static StringToken FromKeyString(string key)
        {
            return new StringToken(null)
            {
                DefaultText = null
            };
        }

        // ToString() on StringToken delegates to a 
        // static Localizer class to look up the 
        // localized text for this StringToken.
        // Putting the lookup in ToString() made
        // our code shring quite a bit.
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


    // This is the main "shim" interface for looking up
    // localized string values and header information
    public interface ILocalizationProvider
    {
        string TextFor(StringToken token);
        LocalizedHeader HeaderFor(PropertyInfo property);

        // This is effectively the same method as above,
        // but PropertyToken is just a class that stands in
        // for a PropertyInfo
        LocalizedHeader HeaderFor(PropertyToken property);
    }

    public class LocalizedHeader
    {
        private readonly Cache<string, string> _properties = new Cache<string, string>();

        public string Heading { get; set; }

        // I'm thinking to leave this bit of flexibility in here for later.
        // Properties might be things like the "caption" or "title," or a help
        // topic.  
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

    // Static Facade over the localization subsystem for convenience.
    // Yes, I know, I have a kneejerk reaction to the word "static"
    // too.  More about that later
    public static class Localizer
    {
        private static Func<ILocalizationProvider> _providerSource = 
            () => new NulloLocalizationProvider();

        // Localizer just uses an ILocalizationProvider under the covers
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

    public class WidgetKeys : StringToken
    {
        public static readonly StringToken WEBSITE = new WidgetKeys( "Website");
        public static readonly StringToken REPORT = new WidgetKeys("Report");
        public static readonly StringToken RSS = new WidgetKeys("RSS Feed");

        protected WidgetKeys(string defaultValue)
            : base(defaultValue)
        {
        }
    }
}