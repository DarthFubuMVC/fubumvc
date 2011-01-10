using FubuCore.Binding;
using FubuMVC.Core.Configuration;
using Microsoft.Practices.ServiceLocation;
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
            For<IValueConverterRegistry>().Use<ValueConverterRegistry>();
            For<ITypeDescriptorCache>().Use<TypeDescriptorCache>();
            ForSingletonOf<IPropertyBinderCache>().Use<PropertyBinderCache>();
            ForSingletonOf<ICollectionTypeProvider>().Use<DefaultCollectionTypeProvider>();
            ForSingletonOf<IModelBinderCache>().Use<ModelBinderCache>();

            For<IModelBinder>().Use<StandardModelBinder>();
        }
    }
}