using System;
using FubuCore.Configuration;
using StructureMap.Configuration.DSL;

namespace FubuMVC.StructureMap.Settings
{
    public class SettingRegistry : Registry
    {
        public SettingRegistry()
        {
            For<ISettingsProvider>().Use<SettingsProvider>();
            For<ISettingsSource>().Add(new AppSettingsSettingSource(SettingCategory.core));
        }

        public void AddSettingType<T>() where T : class, new()
        {
            ForSingletonOf<T>().Use(x => x.GetInstance<ISettingsProvider>().SettingsFor<T>());
        }

        public void AddSettingType(Type type)
        {
            For(type).Singleton().Use(c => c.GetInstance<ISettingsProvider>().SettingsFor(type));
        }
    }
}