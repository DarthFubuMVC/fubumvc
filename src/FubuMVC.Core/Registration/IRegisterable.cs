using System;
using FubuMVC.Core.Registration.ObjectGraph;
using StructureMap.Configuration.DSL;

namespace FubuMVC.Core.Registration
{
    public interface IRegisterable
    {
        void Register(Action<Type, ObjectDef> action);
        void Register(Registry registry);
    }
}