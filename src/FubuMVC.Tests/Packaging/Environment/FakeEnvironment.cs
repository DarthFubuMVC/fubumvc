using System;
using System.Collections.Generic;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Packaging.Environment;

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