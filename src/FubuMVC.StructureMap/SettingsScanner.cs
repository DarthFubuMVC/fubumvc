using System;
using FubuCore.Configuration;
using StructureMap;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;

namespace FubuMVC.StructureMap
{
    public class SettingsScanner : IRegistrationConvention
    {
        public static readonly Func<Type, bool> DefaultFilter =
            type => type.Name.EndsWith("Settings") && !type.IsInterface;

        private readonly Func<Type, bool> _filter;

        public SettingsScanner()
            : this(DefaultFilter)
        {
        }

        public SettingsScanner(Func<Type, bool> filter)
        {
            _filter = filter;
        }

        public void Process(Type type, Registry graph)
        {
            if (!_filter(type)) return;

            graph.For(type).LifecycleIs(InstanceScope.Singleton).Use(c => c.GetInstance<ISettingsProvider>().SettingsFor(type));
        }
    }
}