using System;
using FubuMVC.Core.Packaging;
using StructureMap.Configuration.DSL;

namespace FubuFastPack.StructureMap
{
    public static class ContainerActivatorExtensions
    {
        public static void Activate<T>(this Registry registry, string description, Action<T> activation)
        {
            registry.For<IActivator>().Add<LambdaActivator<T>>()
                .Ctor<string>().Is(description)
                .Ctor<Action<T>>().Is(activation);
        }
    }
}