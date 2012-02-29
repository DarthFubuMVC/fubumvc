using System;
using System.Collections.Generic;
using Bottles;
using Bottles.Environment;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Bootstrapping
{
    public interface IContainerFacility
    {
        IBehaviorFactory BuildFactory();
        void Register(Type serviceType, ObjectDef def);

        void Inject(Type abstraction, Type concretion);
        
        // TODO -- just get rid of these methods now that we broke down and did GetAll<T>()
        IEnumerable<IActivator> GetAllActivators();
        IEnumerable<IInstaller> GetAllInstallers();

        T Get<T>();
        IEnumerable<T> GetAll<T>();
    }
}