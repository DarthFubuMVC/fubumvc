using System;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime;
using StructureMap.Pipeline;

namespace FubuMVC.Core.Bootstrapping
{
    public interface IContainerFacility
    {
        IServiceFactory BuildFactory(BehaviorGraph graph);
        void Register(Type serviceType, Instance instance);

        void Shutdown();
    }
}