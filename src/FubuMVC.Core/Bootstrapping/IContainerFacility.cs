using System;
using System.Collections.Generic;
using Bottles;
using Bottles.Environment;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Bootstrapping
{
    public interface IContainerFacility
    {
        IBehaviorFactory BuildFactory(DiagnosticLevel diagnosticLevel);
        void Register(Type serviceType, ObjectDef def);
        IEnumerable<IActivator> GetAllActivators();
        IEnumerable<IInstaller> GetAllInstallers();
        T Get<T>() where T : class;
    }

    public static class ContainerFacilityExtensions
    {
        public static void SpinUp(this IContainerFacility facility)
        {
    
        }
    }

    

}