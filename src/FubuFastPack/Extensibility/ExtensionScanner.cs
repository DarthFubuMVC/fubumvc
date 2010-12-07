using System;
using FubuCore;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;

namespace FubuFastPack.Extensibility
{
    public class ExtensionScanner : IRegistrationConvention
    {
        public void Process(Type type, Registry graph)
        {
            if (type.IsConcrete() && type.BaseType.IsGenericType && type.BaseType.GetGenericTypeDefinition() == typeof(Extends<>))
            {
                var entityType = type.BaseType.GetGenericArguments()[0];
                ExtensionProperties.Register(entityType, type);
            }
        }
    }
}