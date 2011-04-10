using System;
using System.Collections.Generic;
using Bottles.Diagnostics;

namespace Bottles.Environment
{
    public interface IEnvironment : IDisposable
    {
        IEnumerable<IInstaller> StartUp(IPackageLog log);
    }
}