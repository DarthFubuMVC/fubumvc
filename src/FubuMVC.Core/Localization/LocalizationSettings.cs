using System.Globalization;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core.Localization
{
    public class LocalizationSettings : IFeatureSettings
    {
        public bool Enabled { get; set; }
        public CultureInfo DefaultCulture { get; set; }

        void IFeatureSettings.Apply(FubuRegistry registry)
        {
            if (!Enabled) return;
            
            registry.Services<BasicLocalizationServices>();
            if (DefaultCulture != null)
            {
                registry.Services(x => x.ReplaceService(DefaultCulture));
            }
        }
    }
}