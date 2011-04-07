using System;
using System.Collections.Generic;
using Bottles.Diagnostics;
using Bottles.Environment;
using FubuMVC.Core.Packaging;

namespace FubuMVC.Tests.Packaging.Environment
{
    public class FakeEnvironment : IEnvironment
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IInstaller> StartUp(IPackageLog log)
        {
            throw new NotImplementedException();
        }
    }
}