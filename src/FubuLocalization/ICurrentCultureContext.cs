using System.Globalization;

namespace FubuLocalization
{
    public interface ICurrentCultureContext
    {
        CultureInfo CurrentCulture { get; set; }
        CultureInfo CurrentUICulture { get; set; }
    }
}