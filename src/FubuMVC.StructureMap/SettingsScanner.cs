using System;
using FubuCore.Configuration;
using StructureMap;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;

namespace FubuMVC.StructureMap
{
    public class SettingsScanner : IRegistrationConvention
    {
        public void Process(Type type, Registry graph)
        {
            if (!type.Name.EndsWith("Settings") || type.IsInterface) return;

            graph.For(type).LifecycleIs(InstanceScope.Singleton).Use(c => c.GetInstance<ISettingsProvider>().SettingsFor(type));
        }
    }
}