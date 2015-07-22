using System;
using StructureMap.Pipeline;

namespace FubuMVC.Core.Registration
{
    public interface IRegisterable
    {
        void Register(Action<Type, Instance> action);
    }
}