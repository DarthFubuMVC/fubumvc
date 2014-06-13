using System;
using FubuCore;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace FubuMVC.StructureMap.Settings
{
    public class SettingPolicy : IFamilyPolicy
    {
        public PluginFamily Build(Type type)
        {
            if (type.Name.EndsWith("Settings") && type.IsConcreteWithDefaultCtor())
            {
                var family = new PluginFamily(type);
                var instance = buildInstanceForType(type);
                family.SetDefault(instance);

                return family;
            }

            return null;
        }

        public bool AppliesToHasFamilyChecks
        {
            get { return true; }
        }


        private static Instance buildInstanceForType(Type type)
        {
            var instanceType = typeof (SettingsInstance<>).MakeGenericType(type);
            var instance = Activator.CreateInstance(instanceType).As<Instance>();
            return instance;
        }
    }
}