using System;
using FubuMVC.Core.Packaging;
using StructureMap;
using StructureMap.Configuration.DSL;

namespace FubuFastPack.StructureMap
{
    public static class ContainerExtensions
    {
        public static void Activate<T>(this Registry registry, string description, Action<T> activation)
        {
            registry.For<IActivator>().Add<LambdaActivator<T>>()
                .Ctor<string>().Is(description)
                .Ctor<Action<T>>().Is(activation);
        }

        public static void Activate(this Registry registry, string description, Action activation)
        {
            registry.For<IActivator>().Add<LambdaActivator>()
                .Ctor<string>().Is(description)
                .Ctor<Action>().Is(activation);
        }

        public static void ExecuteInTransaction<T>(this IContainer container, Action<T> action)
        {
            container.GetInstance<TransactionProcessor>().Execute(action);
        }
    }
}