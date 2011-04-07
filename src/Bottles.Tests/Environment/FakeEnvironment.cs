using System;
using System.Collections.Generic;
using Bottles.Diagnostics;
using Bottles.Environment;

namespace Bottles.Tests.Environment
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