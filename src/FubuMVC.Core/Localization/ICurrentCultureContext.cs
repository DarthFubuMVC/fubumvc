using System.Globalization;

namespace FubuMVC.Core.Localization
{
    public interface ICurrentCultureContext
    {
        CultureInfo CurrentCulture { get; set; }
        CultureInfo CurrentUICulture { get; set; }
    }
}