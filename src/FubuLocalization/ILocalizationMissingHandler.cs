using System.Globalization;

namespace FubuLocalization
{
    public interface ILocalizationMissingHandler
    {
        string FindMissingText(StringToken key, CultureInfo culture);
        string FindMissingProperty(PropertyToken key, CultureInfo culture);
        string FindMissingText(string key, string defaultValue, CultureInfo culture);
    }
}