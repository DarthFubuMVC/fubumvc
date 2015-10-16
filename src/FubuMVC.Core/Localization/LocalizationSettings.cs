using System.Globalization;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core.Localization
{
    public class LocalizationSettings : IFeatureSettings
    {
        public LocalizationSettings()
        {
            DefaultCulture = CultureInfo.CurrentCulture;
        }

        public bool Enabled { get; set; }
        public CultureInfo DefaultCulture { get; set; }

        void IFeatureSettings.Apply(FubuRegistry registry)
        {
            if (!Enabled) return;
            
            registry.Services.IncludeRegistry<BasicLocalizationServices>();
            if (DefaultCulture != null)
            {
                registry.Services.ReplaceService(DefaultCulture);
            }
        }
    }
}