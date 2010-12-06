using System;
using System.Collections.Generic;

namespace FubuMVC.Core.Packaging
{
    public class LambdaBootstrapper : IBootstrapper
    {
        private readonly Func<IPackageLog, IEnumerable<IPackageActivator>> _bootstrapper;

        public LambdaBootstrapper(Func<IPackageLog, IEnumerable<IPackageActivator>> bootstrapper)
        {
            _bootstrapper = bootstrapper;
        }

        public IEnumerable<IPackageActivator> Bootstrap(IPackageLog log)
        {
            return _bootstrapper(log);
        }
    }
}