using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FubuLocalization;
using FubuLocalization.Basic;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.UI;

namespace FubuMVC.Localization
{
    public class BasicLocalizationSupport : IFubuRegistryExtension
    {
        private readonly IList<Action<ServiceRegistry>> _modifications = new List<Action<ServiceRegistry>>();

        public CultureInfo DefaultCulture
        {
            set
            {
                _modifications.Add(x => x.ReplaceService(value));
            }
        }

        void IFubuRegistryExtension.Configure(FubuRegistry registry)
        {
            registry.Services<BasicLocalizationServices>();

            if(_modifications.Any())
            {
                registry.Services(x => _modifications.Each(modify => modify(x)));
            }

        }

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

                SetServiceIfNone<ILocalizationCache, LocalizationCache>();
                SetServiceIfNone<ILocalizationMissingHandler, LocalizationMissingHandler>();
                SetServiceIfNone<ILocalizationProviderFactory, LocalizationProviderFactory>();
                SetServiceIfNone<ILocalizationStorage, BottleAwareXmlLocalizationStorage>();

                AddService<IActivator, SpinUpLocalizationCaches>();
            }
        }

    }
}
