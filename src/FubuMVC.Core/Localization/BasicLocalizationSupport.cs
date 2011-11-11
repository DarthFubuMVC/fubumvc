using System.Globalization;
using Bottles;
using FubuCore;
using FubuLocalization;
using FubuLocalization.Basic;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Core.Localization
{
    public class BasicLocalizationSupport : IFubuRegistryExtension
    {
        private readonly FubuPackageRegistry _internalRegistry = new FubuPackageRegistry();
        private bool _useTheDefaultStorageMechanism = true;
        private ObjectDef _localizationLoader;

        public BasicLocalizationSupport()
        {
            _internalRegistry.Services(x =>
            {
                x.SetServiceIfNone(new CultureInfo("en-US"));
                x.SetServiceIfNone<ILocalizationCache, LocalizationCache>();
                x.SetServiceIfNone<ILocalizationMissingHandler, LocalizationMissingHandler>();
                x.SetServiceIfNone<ILocalizationProviderFactory, LocalizationProviderFactory>();
            });

            _internalRegistry.HtmlConvention(x => x.Labels.Builder<LabelBuilder>());
            
        }

        public CultureInfo DefaultCulture
        {
            set
            {
                _internalRegistry.Services(x => x.ReplaceService(value));
            }
        }

        void IFubuRegistryExtension.Configure(FubuRegistry registry)
        {
            _internalRegistry.As<IFubuRegistryExtension>().Configure(registry);

            registry.Services(x =>
            {
                if (_useTheDefaultStorageMechanism)
                {
                    x.AddService<IActivator, RegisterXmlDirectoryLocalizationStorage>();
                }

                if (_localizationLoader != null)
                {
                    x.AddService(typeof (IActivator), _localizationLoader);
                }

                x.AddService<IActivator, SpinUpLocalizationCaches>();
            });

            
        }

        public void LoadLocalizationWith<T>() where T : IActivator
        {
            _localizationLoader = ObjectDef.ForType<T>();
            _useTheDefaultStorageMechanism = false;
        }

        public void LocalizationStorageIs<T>() where T : ILocalizationStorage
        {
            _internalRegistry.Services(x => x.ReplaceService<ILocalizationStorage, T>());
            _useTheDefaultStorageMechanism = false;
        }
    }
}