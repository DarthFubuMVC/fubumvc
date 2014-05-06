using System;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Bootstrapping
{
    public interface IContainerFacility
    {
        IServiceFactory BuildFactory(BehaviorGraph graph);
        void Register(Type serviceType, ObjectDef def);

        void Shutdown();
    }
}