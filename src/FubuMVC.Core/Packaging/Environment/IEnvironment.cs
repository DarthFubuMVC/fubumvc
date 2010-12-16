using System;
using System.Collections.Generic;

namespace FubuMVC.Core.Packaging.Environment
{
    public interface IEnvironment : IDisposable
    {
        IEnumerable<IInstaller> StartUp(IPackageLog log);
    }
}