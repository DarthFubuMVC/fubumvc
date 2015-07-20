using System.Globalization;

namespace FubuMVC.Core.Localization
{
    public class LocalizationSettings
    {
        public bool Enabled { get; set; }
        public CultureInfo DefaultCulture { get; set; }
    }
}