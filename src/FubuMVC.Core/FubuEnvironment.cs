using System.Collections.Generic;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Packaging.Environment;

namespace FubuMVC.Core
{
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