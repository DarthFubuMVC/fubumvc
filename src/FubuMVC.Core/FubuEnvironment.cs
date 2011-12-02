using System.Collections.Generic;
using Bottles.Diagnostics;
using Bottles.Environment;

namespace FubuMVC.Core
{
    // TODO -- alter in favor of using the IApplicationSource
    public abstract class FubuEnvironment : IEnvironment
    {
        public virtual void Dispose() {}

        public IEnumerable<IInstaller> StartUp(IPackageLog log)
        {
            var application = createApplication();            
            application.Bootstrap();
            return application.GetAllInstallers();
        }

        protected abstract FubuApplication createApplication();
    }
}