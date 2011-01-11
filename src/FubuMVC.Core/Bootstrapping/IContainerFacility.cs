using System;
using System.Collections.Generic;
using System.Web.Routing;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Packaging.Environment;
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
    }
}