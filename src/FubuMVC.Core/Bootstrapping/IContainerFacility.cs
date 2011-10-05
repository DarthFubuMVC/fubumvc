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
        IEnumerable<IActivator> GetAllActivators();
        IEnumerable<IInstaller> GetAllInstallers();
        T Get<T>();
        IEnumerable<T> GetAll<T>();
    }

    public static class ContainerFacilityExtensions
    {
        public static void SpinUp(this IContainerFacility facility)
        {
    
        }
    }

    

}