using System;
using System.Collections.Generic;

namespace FubuMVC.Core.Packaging
{
    public class LambdaBootstrapper : IBootstrapper
    {
        private readonly Func<IPackageLog, IEnumerable<IActivator>> _bootstrapper;

        public LambdaBootstrapper(Func<IPackageLog, IEnumerable<IActivator>> bootstrapper)
        {
            _bootstrapper = bootstrapper;
        }

        public IEnumerable<IActivator> Bootstrap(IPackageLog log)
        {
            return _bootstrapper(log);
        }
    }
}