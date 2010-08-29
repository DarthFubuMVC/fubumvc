using System.Reflection;

namespace FubuCore.Localization
{
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
}