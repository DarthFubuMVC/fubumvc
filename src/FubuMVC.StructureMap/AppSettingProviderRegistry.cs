using FubuCore;
using FubuCore.Binding;
using FubuCore.Binding.InMemory;
using FubuCore.Configuration;
using FubuCore.Conversion;
using FubuCore.Reflection;
using FubuMVC.StructureMap;
using StructureMap.Configuration.DSL;

namespace FubuMVC.StructureMap
{
    public class AppSettingProviderRegistry : Registry
    {
        public AppSettingProviderRegistry()
        {
            For<ISettingsProvider>().Use<AppSettingsProvider>();
            For<IObjectResolver>().Use<ObjectResolver>();
            For<IServiceLocator>().Use<StructureMapServiceLocator>();
            For<ITypeDescriptorCache>().Use<TypeDescriptorCache>();
            For<IBindingLogger>().Use<NulloBindingLogger>();
            For<IObjectConverter>().Use<ObjectConverter>();
        }
    }
}