using System;
using FubuMVC.Core.Configuration;
using FubuMVC.Core.Models;
using FubuMVC.Core.Runtime;
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
            ForSingletonOf<IModelBinderCache>().Use<ModelBinderCache>();
        }
    }
}