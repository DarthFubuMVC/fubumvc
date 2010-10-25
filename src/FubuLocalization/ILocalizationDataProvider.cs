using System.Globalization;
using System.Reflection;

namespace FubuLocalization
{
    public interface ILocalizationDataProvider
    {
        CultureInfo Culture { get; }
        string GetTextForKey(StringToken key);
        string GetHeader(PropertyInfo property);
        string GetHeader(PropertyToken property);
        string GetDefaultText(PropertyToken property);
    }
}