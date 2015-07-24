using System.Globalization;
using FubuLocalization;
using FubuLocalization.Basic;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core.Localization
{
    public class BasicLocalizationServices : ServiceRegistry
    {
        public BasicLocalizationServices()
        {
            SetServiceIfNone(new CultureInfo("en-US"));
            SetServiceIfNone<ICurrentCultureContext>(new CurrentCultureContext
            {
                CurrentCulture = new CultureInfo("en-US"),
                CurrentUICulture = new CultureInfo("en-US")
            });

            SetServiceIfNone<ILocalizationCache, LocalizationCache>().Singleton();
            SetServiceIfNone<ILocalizationMissingHandler, LocalizationMissingHandler>();
            SetServiceIfNone<ILocalizationProviderFactory, LocalizationProviderFactory>();
            SetServiceIfNone<ILocalizationStorage, PackageAwareXmlLocalizationStorage>();

            AddService<IActivator, SpinUpLocalizationCaches>();
        }
    }
}