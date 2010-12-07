﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace FubuMVC.Core.Packaging
{
    public interface IPackageFacility
    {
        void Assembly(Assembly assembly);
        void Bootstrapper(IBootstrapper bootstrapper);
        void Loader(IPackageLoader loader);
        void Facility(PackageFacility facility);
        void Activator(IPackageActivator activator);

        void Bootstrap(Func<IPackageLog, IEnumerable<IPackageActivator>> lambda);
    }
}