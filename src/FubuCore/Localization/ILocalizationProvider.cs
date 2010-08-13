using System.Reflection;

namespace FubuCore.Localization
{
    public interface ILocalizationProvider
    {
        string TextFor(StringToken token);
        LocalizedHeader HeaderFor(PropertyInfo property);

        // This is effectively the same method as above,
        // but PropertyToken is just a class that stands in
        // for a PropertyInfo
        LocalizedHeader HeaderFor(PropertyToken property);
    }
}