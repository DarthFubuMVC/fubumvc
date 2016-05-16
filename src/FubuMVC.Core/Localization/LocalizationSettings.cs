using System.Globalization;
using FubuCore.Descriptions;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core.Localization
{
    public class LocalizationSettings : IFeatureSettings, DescribesItself
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

        public void Describe(Description description)
        {
            description.ShortDescription = "Localization Configuration";
            description.Properties[nameof(DefaultCulture)] = DefaultCulture.ToString();
            description.Properties[nameof(Enabled)] = Enabled.ToString();
        }
    }
}