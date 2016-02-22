using System.Globalization;

namespace FubuMVC.Core.Localization
{
    // SAMPLE: MissingHandler
    public interface ILocalizationMissingHandler
    {
        string FindMissingText(StringToken key, CultureInfo culture);
        string FindMissingProperty(PropertyToken property, CultureInfo culture);
    }
    // ENDSAMPLE
}