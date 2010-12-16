using System.Collections.Generic;
using System.Web.Routing;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Packaging.Environment;

namespace FubuMVC.Core
{
    public abstract class FubuEnvironment : IEnvironment
    {
        public virtual void Dispose()
        {
        }

        public IEnumerable<IInstaller> StartUp(IPackageLog log)
        {
            var application = createApplication();
            application.Bootstrap(new List<RouteBase>());

            return application.Facility.GetAllInstallers();
        }

        protected abstract FubuApplication createApplication();
    }
}