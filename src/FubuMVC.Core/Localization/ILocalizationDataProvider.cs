using System.Globalization;
using System.Reflection;

namespace FubuMVC.Core.Localization
{
    public interface ILocalizationDataProvider
    {
        CultureInfo Culture { get; }
        string GetTextForKey(StringToken key);
        string GetHeader(PropertyInfo property);
        string GetHeader(PropertyToken property);
    }
}